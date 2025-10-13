import React, { useEffect, useState, lazy, Suspense } from "react";
import { Link } from "react-router-dom";
import Sidebar from "../components/Sidebar";

// Lazy load components that are not immediately needed
const SearchResultCard = lazy(() => import("../components/SearchResultCard"));
const FriendCard = lazy(() => import("../components/FriendCard"));

export default function FriendsPage() {
    const [friends, setFriends] = useState([]);
    const [searchTerm, setSearchTerm] = useState("");
    const [searchResults, setSearchResults] = useState([]);

    useEffect(() => {
        const fetchFriends = async () => {
            try {
                const res = await fetch(`/api/friends`, {
                    headers: { Authorization: `Bearer ${localStorage.getItem("token")}` }
                });
                const data = await res.json();
                // console.log("[DEBUG] Fetched friends data:", data);
                setFriends(data.friends);
            } catch (err) {
                console.error("Failed to fetch friends:", err);
            }
        };
        fetchFriends();
    }, []);

    const handleUnfollow = async (userId) => {
        try {
            const res = await fetch(`/api/friends/unfollow/${userId}`, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${localStorage.getItem("token")}`
                }
            });

            if (res.ok) {
                setFriends(prev => prev.filter(f => f.userId !== userId));
            } else {
                console.error("Failed to unfollow user.");
            }
        } catch (err) {
            console.error("Error unfollowing user:", err);
        }
    };

    const handleSearch = async () => {
        if (!searchTerm.trim()) return;

        const res = await fetch(`/api/user/search?query=${encodeURIComponent(searchTerm)}`, {
            headers: {
                Authorization: `Bearer ${localStorage.getItem("token")}`
            }
        });
        const data = await res.json();
        setSearchResults(data);
    };

    const handleFollow = async (userId) => {
        const res = await fetch(`/api/friends/follow/${userId}`, {
            method: 'POST',
            headers: {
                Authorization: `Bearer ${localStorage.getItem("token")}`
            }
        });

        if (res.ok) {
            alert("Follow request sent!");
        } else {
            const err = await res.text();
            alert("Failed to follow: " + err);
        }
    };

    const filteredFriends = Array.isArray(friends)
        ? friends.filter(f =>
            (f.userName?.toLowerCase() || '').includes(searchTerm.toLowerCase())
        )
        : [];

    return (
        <div className="dashboard-container">
            <Sidebar />
            <div className="dashboard-main">
                <h1>My Friends</h1>
                <input
                    type="text"
                    placeholder="Search friends..."
                    value={searchTerm}
                    onChange={e => setSearchTerm(e.target.value)}
                />
                <button onClick={handleSearch}>Search</button>

                <Suspense fallback={<p>Loading search results...</p>}>
                    <div className="search-results">
                        {searchResults.map(user => (
                            <SearchResultCard
                                key={user.id}
                                user={user}
                                onFollow={handleFollow}
                            />
                        ))}
                    </div>
                </Suspense>

                <Suspense fallback={<p>Loading friends...</p>}>
                    <div className="friend-grid">
                        {filteredFriends.length === 0 ? (
                            <p>No friends found.</p>
                        ) : (
                            filteredFriends.map(friend => (
                                <FriendCard
                                    key={friend.userId}
                                    friend={friend}
                                    onUnfollow={handleUnfollow}
                                />
                            ))
                        )}
                    </div>
                </Suspense>
            </div>
        </div>
    );
}
