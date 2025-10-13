import { useParams } from "react-router-dom";
import { useEffect, useState, lazy, Suspense } from "react";
import "../index.css";

const Sidebar = lazy(() => import('../components/Sidebar'));

const Books = () => {
    const [search, setSearch] = useState('');
    const [results, setResults] = useState([]);
    const [loading, setLoading] = useState(false);
    const [message, setMessage] = useState('');

    const fetchBooks = async () => {
        setLoading(true);
        setMessage('');
        try {
            const books = await HardcoverService.searchBooks(search);
            setResults(books);
        } catch (err) {
            console.error('Fetch error:', err);
            setMessage('Failed to fetch books.');
        } finally {
            setLoading(false);
        }
    };

    const addBook = async (book) => {
        const bookDto = {
            Id: book.id,
            Title: book.title,
            CachedContributors: book.cached_contributors,
            CoverUrl: book.cover_url,
            Slug: book.slug
        };
    
        try {
            const response = await fetch('/api/books', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(bookDto)
            });
    
            if (response.ok) {
                setMessage(`✅ "${book.title}" added to your library!`);
            } else if (response.status === 409) {
                setMessage(`⚠️ "${book.title}" is already in your library.`);
            } else {
                setMessage(`❌ Failed to add "${book.title}".`);
            }
        } catch (err) {
            console.error('Backend add error:', err);
            setMessage('An error occurred while adding the book.');
        }
    };

    return (
        <div className="dashboard-container">
            <Suspense fallback={<div>Loading menu...</div>}>
                <Sidebar />
            </Suspense>

            <div className="book-details-container">
                <div className="book-details-card">
                    <img
                        loading="lazy"
                        src={book.coverUrl || '/placeholder-book-cover.png'}
                        alt={book.title}
                    />

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
                    </div>
                </div>
            </div>
        </div>
    );
};

export default Books;
