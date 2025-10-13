export async function searchBooksByTitle(title) {
    if (!title || title.trim() === '') return [];

    const token = localStorage.getItem('token');

    const res = await fetch(`/api/books/search-hardcover?title=${encodeURIComponent(title)}`, {
        headers: {
            'Authorization': `Bearer ${token}`
        }
    });

    if (!res.ok) {
        throw new Error("Backend failed to fetch from Hardcover");
    }

    const json = await res.json();
    return json;
}



export async function addBookToUser(bookId) {
    const jwtToken = localStorage.getItem("token");

    const response = await fetch(`/api/books/import`, {
        method: 'POST',
        headers: {
            'Authorization': `Bearer ${jwtToken}`,
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ id: bookId }) 
    });

    if (!response.ok) {
        throw new Error("Failed to add book to user library.");
    }

    return await response.json();
}
