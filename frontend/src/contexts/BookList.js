import { useEffect, useState } from "react"
import API from '../api/client'
import BookCard from './BookCard'
import { useAuth } from "./AuthContext"


export default function BookList() {
    const [books, setBooks] = useState([])
    const { user } = useAuth

    useEffect(() => {
        const fetchBooks = async () => {
            try {
                const response = await API.get('/books')
                setBooks(response.data)
            } catch (err) {
                console.error('Failed to load books:', err)
            }
        }
        fetchBooks()
    }, [])

    return (
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            {books.map(book => (
                <BookCard
                    key={book.id}
                    book={book}
                    isLoggedIn={!!user}
                />
            ))}
        </div>
    )
}