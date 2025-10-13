import { useState, useEffect } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';
import { auth } from '../api/endpoints';
import '../index.css';

export default function Login() {
    const [identifier, setIdentifier] = useState('');
    const [password, setPassword] = useState('');
    const [isLoading, setIsLoading] = useState(false);
    const navigate = useNavigate();

    // Redirect to dashboard if already logged in
    useEffect(() => {
        if (localStorage.getItem('token')) {
            console.log("Navigating to dashboard...");
            navigate('/dashboard');
        }
    }, [navigate]);

    // Handle login form submit
    const handleSubmit = async (e) => {
        e.preventDefault();

        if (!identifier || !password) {
            toast.error('Please fill in all fields');
            return;
        }

        setIsLoading(true);

        try {
            const response = await auth.login(identifier, password);
            const token = response.data.token;

            if (!token) {
                toast.error("No token returned from login.");
                return;
            }

            localStorage.setItem('token', token);
            toast.success('Login successful');

            setTimeout(() => {
                navigate('/dashboard');
            }, 100); // slight delay ensures token is saved
        } catch (err) {
            console.error("Login error:", err.response?.data || err.message);
            const errorMessage = err.response?.data?.message || 'Login failed. Please try again';
            toast.error(errorMessage);
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <div className="auth-page">
            <h1>Login</h1>
            <form onSubmit={handleSubmit} aria-label="Login Form">
                <div className="form-group">
                    <label htmlFor="identifier">Email or Username</label>
                    <input
                        type="text"
                        id="identifier"
                        value={identifier}
                        onChange={(e) => setIdentifier(e.target.value)}
                        placeholder="Enter your email or username"
                        disabled={isLoading}
                        autoFocus
                        aria-required="true"
                    />
                </div>

                <div className="form-group">
                    <label htmlFor="password">Password</label>
                    <input
                        type="password"
                        id="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        placeholder="Enter your password"
                        disabled={isLoading}
                        aria-required="true"
                    />
                </div>

                <button
                    type="submit"
                    className="btn primary"
                    disabled={isLoading}
                    aria-busy={isLoading}
                >
                    {isLoading ? (
                        <span className="spinner"></span>
                    ) : (
                        'Login'
                    )}
                </button>
            </form>

            <div className="auth-links">
                <p>
                    Don’t have an account? <Link to="/signup">Sign up here</Link>
                </p>
                <Link to="/forgot-password">Forgot Password?</Link>
            </div>
        </div>
    );
}
