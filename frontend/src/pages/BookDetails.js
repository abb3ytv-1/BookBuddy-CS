import { useParams } from "react-router-dom";
import { useEffect, useState, lazy, Suspense } from "react";
import '../index.css';

const Sidebar = lazy(() => import("../components/Sidebar"));

const BookDetails = () => {
    const { id } = useParams();
    const [book, setBook] = useState(null);
    const [review, setReview] = useState('');
    const [rating, setRating] = useState('');
    const [message, setMessage] = useState('');

    // Load book details on mount
    useEffect(() => {
        const fetchBook = async () => {
            try {
                const token = localStorage.getItem("token");
                const res = await fetch(`/api/books/${id}`, {
                    headers: {
                        "Authorization": `Bearer ${token}`
                    }
                });

                if (!res.ok) throw new Error("Failed to fetch book");

                const data = await res.json();
                setBook(data);
            } catch (err) {
                console.error("Failed to load book", err);
            }
        };
        fetchBook();
    }, [id]);

    const handleReviewChange = (e) => {
        setReview(e.target.value);
    };

    const handleRatingChange = (e) => {
        setRating(e.target.value);
    };

    const submitReview = async () => {
        const jwtToken = localStorage.getItem("token");

        if (!book?.userBookId == null) {
            setMessage("❌ Book is not in your library.");
            return;
        }

        try {
            const res = await fetch(`/api/books/review/${book.userBookId}`, {
                method: "PUT",
                headers: {
                    "Authorization": `Bearer ${jwtToken}`,
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({
                    review,
                    rating: parseInt(rating, 10)
                })
            });

            if (res.ok) {
                setMessage("✅ Review submitted!");
            } else {
                const errorText = await res.text();
                console.error("Review submission failed:", errorText);
                setMessage("❌ Failed to submit review. Please try again.");
            }
        } catch (err) {
            console.error("Submit failed", err);
            setMessage("❌ An unexpected error occurred.");
        }
    };

    if (!book) return <p>Loading...</p>;

    return (
        <div className="dashboard-container">
            <Suspense fallback={<div>Loading Sidebar...</div>}>
                <Sidebar />
            </Suspense>
            <div className="book-details-container">
                <div className="book-details-card">
                    <img loading="lazy" src={book.coverUrl || '/placeholder-book-cover.png'} alt={book.title} />

                    <div className="book-info-section">
                        <h1 className="book-title">{book.title}</h1>
                        <p className="book-author">by {book.author}</p>

                        {book.description && (
                            <p className="book-description">{book.description}</p>
                        )}

                        <div className="book-meta">
                            {book.publicationYear && <p><strong>Year:</strong> {book.publicationYear}</p>}
                            {book.genre && <p><strong>Genre:</strong> {book.genre}</p>}
                            {book.pages && <p><strong>Pages:</strong> {book.pages}</p>}
                        </div>

                        {/* Only render review section if userBookId exists */}
                        {book.userBookId > 0 ? (
                            <div className="review-section">
                                <textarea
                                    placeholder="Write your review..."
                                    value={review}
                                    onChange={handleReviewChange}
                                />
                                <select value={rating} onChange={handleRatingChange}>
                                    <option value="">Rate this book</option>
                                    {[1, 2, 3, 4, 5].map(n => (
                                        <option key={n} value={n}>{n} Star{n > 1 ? 's' : ''}</option>
                                    ))}
                                </select>
                                <button onClick={submitReview}>Submit Review</button>
                                {message && <p>{message}</p>}
                            </div>
                        ) : (
                            <p className="error-message">❌ Book is not in your library.</p>
                        )}
                    </div>
                </div>
            </div>
        </div>
    );
};

export default BookDetails;
