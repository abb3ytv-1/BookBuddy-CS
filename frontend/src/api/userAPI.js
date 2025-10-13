import API from './client';

export const fetchUserProfile = async () => {
    try {
        const token = localStorage.getItem('token');
        console.log('[fetchUserProfile] Token:', token);
        if (!token) throw new Error("No token found");

        const response = await API.get('/user/profile');
        console.log('[fetchUserProfile] Response:', response.data);
        return response.data;
    } catch (error) {
        console.error('[fetchUserProfile] Error:', error.response?.data || error.message);
        throw error;
    }
};

export const fetchDashboardStats = async () => {
    try {
        const token = localStorage.getItem('token');
        if (!token) throw new Error("No token found");

        const response = await API.get('/user/dashboard');
        console.log('[fetchDashboardStats] Response:', response.data);
        return response.data;
    } catch (error) {
        console.error('[fetchDashboardStats] Error:', error.response?.data || error.message);
        throw error;
    }
};


