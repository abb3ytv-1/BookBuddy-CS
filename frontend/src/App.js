import React, { useState, useEffect, lazy, Suspense } from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

import { UserProvider } from './contexts/UserContext';
import { NotificationProvider } from './contexts/NotificationProvider';

// Lazy-loaded pages
const Login = lazy(() => import('./pages/Login'));
const Signup = lazy(() => import('./pages/Signup'));
const Home = lazy(() => import('./pages/Home'));
const Dashboard = lazy(() => import('./pages/Dashboard'));
const ReadingGoals = lazy(() => import('./pages/ReadingGoals'));
const AddBook = lazy(() => import('./pages/AddBook'));
const BookDetails = lazy(() => import('./pages/BookDetails'));
const ForgotPassword = lazy(() => import('./pages/ForgotPassword'));
const ResetPassword = lazy(() => import('./pages/ResetPassword'));
const Achievements = lazy(() => import('./pages/Achievements'));
const FriendsPage = lazy(() => import('./pages/FriendsPage'));
const SettingsPage = lazy(() => import('./pages/Settings'));
const FeedPage = lazy(() => import('./pages/FeedPage'));

// Private Route Component
const PrivateRoute = ({ children }) => {
    const [hasToken, setHasToken] = useState(null);

    useEffect(() => {
        const token = localStorage.getItem('token');
        setHasToken(!!token);
    }, []);

    if (hasToken === null) return null;

    return hasToken ? children : <Navigate to="/login" replace />;
};

function App() {
    return (
        <UserProvider>
            <NotificationProvider>
                <Router>
                    <ToastContainer position='top-center' autoClose={3000} />
                    <Suspense fallback={<div className="loading">Loading...</div>}>
                        <Routes>
                            <Route path="/" element={<Home />} />
                            <Route path="/login" element={<Login />} />
                            <Route path="/signup" element={<Signup />} />
                            <Route path="/dashboard" element={<PrivateRoute><Dashboard /></PrivateRoute>} />
                            <Route path="/reading-goals" element={<PrivateRoute><ReadingGoals /></PrivateRoute>} />
                            <Route path="/add-book" element={<PrivateRoute><AddBook /></PrivateRoute>} />
                            <Route path="/forgot-password" element={<ForgotPassword />} />
                            <Route path="/reset-password/:token" element={<ResetPassword />} />
                            <Route path="/book/:id" element={<BookDetails />} />
                            <Route path="/achievements" element={<PrivateRoute><Achievements /></PrivateRoute>} />
                            <Route path="/friends" element={<PrivateRoute><FriendsPage /></PrivateRoute>} />
                            <Route path="/settings" element={<PrivateRoute><SettingsPage /></PrivateRoute>} />
                            <Route path="/feed" element={<PrivateRoute><FeedPage /></PrivateRoute>} />
                        </Routes>
                    </Suspense>
                </Router>
            </NotificationProvider>
        </UserProvider>
    );
}

export default App;
