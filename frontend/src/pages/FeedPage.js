import React, { useEffect, useState, useRef, useCallback, lazy, Suspense } from "react";
import Sidebar from "../components/Sidebar";

const FeedCard = lazy(() => import("../components/FeedCard"));
const SkeletonCard = lazy(() => import("../components/SkeletonCard"));

export default function FeedPage() {
    const [feed, setFeed] = useState([]);
    const [cursor, setCursor] = useState(null);
    const [hasMore, setHasMore] = useState(true);
    const [loading, setLoading] = useState(false);
    const loadMoreRef = useRef();

    const fetchFeed = useCallback(async () => {
        if (loading || !hasMore) return;

        setLoading(true);
        try {
            const res = await fetch(`/api/feed/social`, {
                headers: {
                    Authorization: `Bearer ${localStorage.getItem("token")}`,
                },
            });

            if (res.ok) {
                const data = await res.json();

                // Format the social posts
                const formatted = data.map((post) => ({
                    id: post.id,
                    username: post.userName,
                    content: post.content,
                }));

                setFeed((prev) => [...prev, ...formatted]);
                setHasMore(false); // No pagination implemented for social feed
            } else {
                console.error("Social feed fetch failed:", res.status);
            }
        } catch (error) {
            console.error("Error fetching social feed:", error);
        } finally {
            setLoading(false);
        }
    }, [loading, hasMore]);


    useEffect(() => {
        fetchFeed();
    }, []);

    // Auto-load next batch when observer element becomes visible
    useEffect(() => {
        if (!loadMoreRef.current || !hasMore || loading) return;

        const observer = new IntersectionObserver(
            (entries) => {
                if (entries[0].isIntersecting) {
                    fetchFeed();
                }
            },
            { threshold: 1 }
        );

        observer.observe(loadMoreRef.current);
        return () => observer.disconnect();
    }, [fetchFeed, hasMore, loading]);

    return (
        <div className="dashboard-container">
            <Sidebar />
            <div className="feed-container" style={{ height: "100vh", overflowY: "auto" }}>
                <h1 style={{ padding: "1rem" }}>📚 Friend Activity Feed</h1>

                <Suspense fallback={<p>Loading feed...</p>}>
                    {feed.map((item) => (
                        <FeedCard key={item.id} data={item} />
                    ))}

                    {loading &&
                        [...Array(3)].map((_, i) => <SkeletonCard key={i} />)}

                    {!loading && feed.length === 0 && (
                        <p style={{ padding: "1rem" }}>Nothing to see here yet.</p>
                    )}

                    {!loading && !hasMore && feed.length > 0 && (
                        <p style={{ padding: "1rem", color: "#888" }}>No more posts.</p>
                    )}
                </Suspense>

                {/* Trigger element for IntersectionObserver */}
                <div ref={loadMoreRef} style={{ height: "1px" }} />
            </div>
        </div>
    );
}
