import React, { useState, lazy, Suspense } from "react";
import { searchBooksByTitle } from '../api/hardcover';
import '../index.css';

const Sidebar = lazy(() => import("../components/Sidebar"));

const AddBook = () => {
    const [search, setSearch] = useState('');
    const [results, setResults] = useState([]);
    const [message, setMessage] = useState('');
    const [loading, setLoading] = useState(false);

    const fetchBooks = async () => {
        if (!search.trim()) {
            setMessage("Please enter a title to search.");
            return;
        }

        setLoading(true);
        setMessage('');
        setResults([]);

        try {
            // Call your API endpoint
            const response = await fetch(`/api/books/search-hardcover?title=${encodeURIComponent(search)}`, {
                headers: {
                    Authorization: `Bearer ${localStorage.getItem("token")}`
                }
            });

            if (!response.ok) {
                if (response.status === 403) {
                    setMessage("❌ Unauthorized. Please check your Hardcover API token.");
                } else {
                    setMessage("❌ Failed to fetch books from Hardcover.");
                }
                return;
            }

            const booksResponse = await response.json();
            console.log('Fetched data:', booksResponse);

            const editions = booksResponse?.books;

            if (!Array.isArray(editions) || editions.length === 0) {
                setMessage('No books found with that title.');
            } else {
                const formatted = editions.map((edition) => ({
                    externalId: edition.id,
                    title: edition.title,
                    author: edition.contributions?.map(c => c.author.name).join(', ') || "Unknown Author",
                    coverUrl: edition.image,
                    slug: edition.slug || '',
                }));

                setResults(formatted);
            }
        } catch (err) {
            console.error('Fetch error:', err);
            setMessage('An error occurred while fetching books.');
        } finally {
            setLoading(false);
        }
    };


    const addBook = async (book) => {
        const jwtToken = localStorage.getItem("token");

        const bookDto = {
        id: book.externalId,
        title: book.title,
        cachedContributors: book.author || "Unknown Author",
        coverUrl: book.coverUrl || '',
        slug: book.slug || '',
        };

        try {
        const response = await fetch('/api/books/import', {
            method: 'POST',
            headers: {
            'Authorization': `Bearer ${jwtToken}`,
            'Content-Type': 'application/json'
            },
            body: JSON.stringify(bookDto),
        });

        if (response.ok) {
            setMessage(`✅ "${book.title}" added to your library!`);
        } else if (response.status === 400 || response.status === 409) {
            setMessage(`⚠️ "${book.title}" is already in your library.`);
        } else {
            setMessage(`❌ Failed to add book.`);
        }
        } catch (err) {
        console.error('Add failed', err);
        setMessage('An error occurred while adding the book.');
        }
    };

    return (
        <div className="dashboard-container">
        <Suspense fallback={<div>Loading Sidebar...</div>}>
            <Sidebar />
        </Suspense>

        <div className="dashboard-main">
            <h1>Search for a Book</h1>
            <input
            type="text"
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            onKeyDown={(e) => {
                if (e.key === 'Enter') {
                fetchBooks();
                }
            }}
            placeholder="Search by title..."
            />

            <button onClick={fetchBooks} disabled={loading}>
            {loading ? 'Searching...' : 'Search'}
            </button>

            {message && <p className="message-box">{message}</p>}

            <div className="book-results">
                {(Array.isArray(results) ? results : []).map((book) => (
                    <div key={book.externalId} className="book-card">
                    <img
                        loading="lazy"
                        src={book.coverUrl || '/placeholder-book-cover.png'}
                        alt={book.title}
                    />
                    <h3>{book.title}</h3>
                    <p>{book.author || 'Unknown Author'}</p>
                    <button onClick={() => addBook(book)}>Add to My Books</button>
                    </div>
                ))}
            </div>

        </div>
        </div>
    );
};

export default AddBook;
