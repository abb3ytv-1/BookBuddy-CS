import api from './client';

export const auth = {
    login: (identifier, password) => api.post('/auth/login', { identifier, password }),
    register: (userData) => api.post('/auth/signup', userData),
    forgotPassword: (email) => api.post('/auth/forgot-password', { email }),
    resetPassword: (token, newPassword) => api.post('/auth/reset-password', { token, newPassword })
};

export const user = {
    getProfile: () => api.get('/user/profile'),
    getDashboardStats: () => api.get('/user/dashboard'),
    updateProfile: (userData) => api.put('/user/profile', userData)
};

export const books = {
    getAll: () => api.get('/books'),
    create: (bookData) => api.post('/books', bookData),
    update: (id, bookData) => api.put(`/books/${id}`, bookData),
    delete: (id) => api.delete(`/books/${id}`)
};