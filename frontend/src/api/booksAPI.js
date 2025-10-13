import API from './client';

// 📚 Fetch all books
export const fetchBooks = async () => {
    try {
        const response = await API.get('/books');
        return response.data;
    } catch (error) {
        console.error('Error fetching books:', error);
        throw new Error('Failed to fetch books');
    }
};

// 🔄 Update status of a book
export const updateStatus = async (bookId, newStatus) => {
    try {
        const response = await API.put(`/books/${bookId}/status`, newStatus);
        return response.data;
    } catch (error) {
        console.error('Error updating book status:', error);
        throw error;
    }
};
