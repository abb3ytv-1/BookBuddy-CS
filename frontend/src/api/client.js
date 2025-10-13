import axios from 'axios';

const api = axios.create({
    // 👇 Note: Add the /api prefix here!
    baseURL: process.env.REACT_APP_API_BASE || 'https://localhost:5224/api',
    headers: {
        'Content-Type': 'application/json'
    },
});

api.interceptors.request.use(config => {
    const token = localStorage.getItem('token');
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});

api.interceptors.response.use(
    (response) => response,
    (error) => {
        if (error.response) {
            if (error.response.status === 401) {
                console.warn("Unauthorized. Redirecting to login...");
                localStorage.removeItem('token');
                window.location.href = '/login';
            } else {
                console.error(`API error: ${error.response.status}`, error.response.data);
            }
        } else {
            console.error("Network error:", error.message);
        }
        return Promise.reject(error);
    }
);

export default api;
