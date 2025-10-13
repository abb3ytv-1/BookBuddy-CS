export default function BookCard({book}){
    const {user} = useAuth()

    return (
        <div className="book-card">
            <h3>{book.title}</h3>
            <p>{book.author}</p>
            {user && (
                <select
                    value={book.status}
                    onChange={(e) => updateStatus(book.id, e.target.value)}
                    >
                        <option value = "WantToRead">Want To Read</option>
                        <option value = "Reading">Reading</option>
                        <option value = "Read">Read</option>
                </select>
            )}
        </div>
    )
}