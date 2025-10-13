import React, { useEffect, useState, lazy, Suspense } from 'react';
import { fetchBooks } from '../api/booksAPI';
import { getUserAchievements } from '../api/achievements';
import { fetchUserProfile, fetchDashboardStats } from '../api/userAPI';
import { useUser } from '../contexts/UserContext';
import { Link } from 'react-router-dom';
import { toast } from 'react-toastify';
import { getSignalRConnection } from '../components/signalRConnection';
import "react-toastify/dist/ReactToastify.css";
import '../index.css';

const Sidebar = lazy(() => import('../components/Sidebar'));

export default function Dashboard() {
    const { user, reloadUser } = useUser();
    const [localUser, setLocalUser] = useState(null);
    const [books, setBooks] = useState([]);
    const [pointGain, setPointGain] = useState(0);
    const [achievements, setAchievements] = useState([]);
    const [loading, setLoading] = useState(true);
    const [reviews, setReviews] = useState({});
    const [ratings, setRatings] = useState({});
    const [dashboardStats, setDashboardStats] = useState({
        booksRead: 0,
        totalBooks: 0,
        points: 0,
        loginStreak: 0
    });

    useEffect(() => {
        const loadData = async () => {
            setLoading(true);
            try {
                await reloadUser();
                const [bookList, achievementList, stats] = await Promise.all([
                    fetchBooks(),
                    getUserAchievements(),
                    fetchDashboardStats()
                ]);
                setBooks(bookList);
                setAchievements(achievementList);
                setDashboardStats(stats);
            } catch (err) {
                console.error("Dashboard load error:", err);
                toast.error("Failed to load dashboard data.");
            } finally {
                setLoading(false);
            }
        };

        loadData();
    }, []);

    useEffect(() => {
        if (user) {
            setLocalUser(user);
        }
    }, [user]);

    const handleStatusChange = async (bookId, newStatus) => {
        const jwtToken = localStorage.getItem("token");

        try {
            const response = await fetch(`/api/books/${bookId}/status`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${jwtToken}`
                },
                body: JSON.stringify({ status: newStatus })
            });

            if (response.ok) {
                const data = await response.json();

                if (data.unlockedAchievements?.length > 0) {
                    data.unlockedAchievements.forEach((ach) => {
                        toast.success(`🎉 Unlocked: ${ach.title}! +${ach.pointsReward} pts`, {
                            autoClose: 4000
                        });
                    });
                }

                setBooks(prevBooks =>
                    prevBooks.map(book =>
                        book.id === bookId ? { ...book, status: data.status } : book
                    )
                );

                await reloadUser();
                const stats = await fetchDashboardStats();
                setDashboardStats(stats);
            } else {
                toast.error("❌ Failed to update book status.");
            }
        } catch (err) {
            console.error("Error updating book status:", err);
            toast.error("⚠️ An error occurred.");
        }
    };

    const handleReviewChange = (userBookId, value) => {
        setReviews(prev => ({ ...prev, [userBookId]: value }));
    };

    const handleRatingChange = (userBookId, value) => {
        setRatings(prev => ({ ...prev, [userBookId]: value }));
    };

    const submitReview = async (userBookId, review, rating) => {
        const jwtToken = localStorage.getItem("token");

        try {
            const res = await fetch(`/api/books/review/${userBookId}`, {
                method: "PUT",
                headers: {
                    "Authorization": `Bearer ${jwtToken}`,
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({ review, rating })
            });

            if (res.ok) {
                toast.success("✅ Review submitted!");
                const stats = await fetchDashboardStats();
                setDashboardStats(stats);
            } else {
                toast.error("❌ Failed to submit review.");
            }
        } catch (err) {
            console.error("Submit failed", err);
            toast.error("An error occurred.");
        }
    };

    if (loading || !localUser) return <div className="loading">Loading dashboard...</div>;

    return (
        <div className="dashboard-container">
            <Suspense fallback={<div className="loading">Loading menu...</div>}>
                <Sidebar />
            </Suspense>

            <div className="dashboard-main">
                <h1 className="section-heading">Welcome back, {localUser.username || 'Reader'}!</h1>
                <img
                    loading="lazy"
                    src={`${localUser.profilePictureUrl || "/default-avatar.png"}?v=${Date.now()}`}
                    alt="Profile"
                    className="profile-picture"
                />
                <p className="profile-bio">{localUser.bio || "This user hasn't written a bio yet."}</p>

                <div className="dashboard-actions">
                    <a href="/add-book" className="btn-add-book">➕ Add Book</a>
                </div>

                <div className="stats-grid">
                    <div className="stat-card">
                        <h3>Books Read</h3>
                        <div className="stat-number">{dashboardStats.booksRead}</div>
                    </div>
                    <div className="stat-card">
                        <h3>Total Books</h3>
                        <div className="stat-number">{dashboardStats.totalBooks}</div>
                    </div>
                    <div className="stat-card">
                        <h3>Points</h3>
                        <div className="stat-number">{dashboardStats.points}</div>
                    </div>
                    {/* If you decide to include loginStreak in your API response: */}
                    {dashboardStats.loginStreak !== undefined && (
                        <div className="stat-card">
                        <h3>Login Streak</h3>
                        <div className="stat-number">{dashboardStats.loginStreak} 🔥</div>
                        </div>
                    )}
                </div>

                <div className="book-collection">
                    <h2>Your Books</h2>
                    <div className="book-grid your-books-grid">
                        {books.map((book) => (
                            <div className="book-card" key={book.id}>
                                <Link to={`/book/${book.id}`} className="book-card-link">
                                    <img loading="lazy" src={book.coverUrl || '/placeholder-book-cover.png'} alt={book.title} />
                                    <div className="book-info">
                                        <h3>{book.title}</h3>
                                        <p>{book.author}</p>
                                    </div>
                                </Link>
                                <div className="book-info">
                                    <p className="book-status">
                                        Status:{" "}
                                        <select
                                            value={book.status}
                                            onClick={(e) => e.stopPropagation()}
                                            onChange={(e) => handleStatusChange(book.id, e.target.value)}
                                        >
                                            <option value="Reading">Reading</option>
                                            <option value="Read">Read</option>
                                            <option value="Unread">Unread</option>
                                        </select>
                                    </p>
                                    {book.status === "Read" && book.userBookId && (
                                        <div className="review-section">
                                            <textarea
                                                value={reviews[book.userBookId] || ""}
                                                onChange={(e) => handleReviewChange(book.userBookId, e.target.value)}
                                                placeholder="Write a review"
                                            />
                                            <select
                                                value={ratings[book.userBookId] || ""}
                                                onChange={(e) => handleRatingChange(book.userBookId, parseInt(e.target.value))}
                                            >
                                                <option value="">Rate this book</option>
                                                {[1, 2, 3, 4, 5].map(n => (
                                                    <option key={n} value={n}>{n} Star{n > 1 ? "s" : ""}</option>
                                                ))}
                                            </select>
                                            <button onClick={(e) => {
                                                e.stopPropagation();
                                                submitReview(book.userBookId, reviews[book.userBookId], ratings[book.userBookId]);
                                            }}>
                                                Save Review
                                            </button>
                                        </div>
                                    )}
                                </div>
                            </div>
                        ))}
                    </div>
                </div>

                <section className='achievement-section'>
                    <h2>🏆 Unlocked Achievements</h2>
                    {achievements.length === 0 ? (
                        <p>No achievements unlocked yet.</p>
                    ) : (
                        <div className='achievement-grid'>
                            {achievements.map((a) => (
                                <div key={a.id} className='achievement-card'>
                                    <img loading="lazy" src={a.iconUrl || 'placeholder-icon.png'} alt={a.title} />
                                    <h3>{a.title}</h3>
                                    <p>{a.description}</p>
                                    <span>+{a.pointsReward}</span>
                                </div>
                            ))}
                        </div>
                    )}
                </section>
            </div>
        </div>
    );
}
