import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';
import '../index.css';

export default function Signup() {
    const [formData, setFormData] = useState({
        username: '',
        email: '',
        password: '',
        confirmPassword: '',
        readingGoal: ''
    });

    const [errors, setErrors] = useState({});
    const [isLoading, setIsLoading] = useState(false);
    const navigate = useNavigate();

    const validateForm = () => {
        const newErrors = {};
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        const usernameRegex = /^[a-zA-Z0-9_]{3,20}$/;

        if (!formData.username) {
            newErrors.username = 'Username is required';
        } else if (!usernameRegex.test(formData.username)) {
            newErrors.username = 'Username must be 3-20 characters (letters, numbers, underscores)';
        }

        if (!formData.email) {
            newErrors.email = 'Email is required';
        } else if (!emailRegex.test(formData.email)) {
            newErrors.email = 'Invalid email format';
        }

        if (!formData.password) {
            newErrors.password = 'Password is required';
        } else if (formData.password.length < 8) {
            newErrors.password = 'Password must be at least 8 characters';
        }

        if (formData.password !== formData.confirmPassword) {
            newErrors.confirmPassword = 'Passwords do not match';
        }

        if (!formData.readingGoal || isNaN(formData.readingGoal) || formData.readingGoal <= 0) {
            newErrors.readingGoal = 'Please enter a valid reading goal';
        }

        setErrors(newErrors);
        return Object.keys(newErrors).length === 0;
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        if (!validateForm()) return;

        setIsLoading(true);

        try {
            const response = await fetch('/api/auth/signup', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    UserName: formData.username,
                    Email: formData.email,
                    Password: formData.password,
                    ReadingGoal: parseInt(formData.readingGoal)
                })
            });

            const data = await response.json();

            if (!response.ok) {
                throw new Error(data.message || 'Signup failed');
            }

            localStorage.setItem('token', data.token);
            toast.success('Account created successfully!');
            navigate('/dashboard');
        } catch (err) {
            toast.error(err.message);
        } finally {
            setIsLoading(false);
        }
    };

    const handleInputChange = (e) => {
        const { name, value } = e.target;
        setFormData(prev => ({ ...prev, [name]: value }));
    };

    return (
        <div className="auth-page">
            <h1>Create Account</h1>
            <form onSubmit={handleSubmit}>
                <div className="form-group">
                    <label>Username</label>
                    <input
                        type="text"
                        name="username"
                        autoFocus
                        value={formData.username}
                        onChange={handleInputChange}
                        className={errors.username ? 'error-input' : ''}
                        placeholder="Enter your username"
                    />
                    {errors.username && <span className="error">{errors.username}</span>}
                </div>

                <div className="form-group">
                    <label>Email</label>
                    <input
                        type="email"
                        name="email"
                        value={formData.email}
                        onChange={handleInputChange}
                        className={errors.email ? 'error-input' : ''}
                        placeholder="Enter your email"
                    />
                    {errors.email && <span className="error">{errors.email}</span>}
                </div>

                <div className="form-group">
                    <label>Password</label>
                    <input
                        type="password"
                        name="password"
                        value={formData.password}
                        onChange={handleInputChange}
                        className={errors.password ? 'error-input' : ''}
                        placeholder="Create a password"
                    />
                    {errors.password && <span className="error">{errors.password}</span>}
                </div>

                <div className="form-group">
                    <label>Confirm Password</label>
                    <input
                        type="password"
                        name="confirmPassword"
                        value={formData.confirmPassword}
                        onChange={handleInputChange}
                        className={errors.confirmPassword ? 'error-input' : ''}
                        placeholder="Confirm your password"
                    />
                    {errors.confirmPassword && <span className="error">{errors.confirmPassword}</span>}
                </div>

                <div className="form-group">
                    <label>Reading Goal (Books per Year)</label>
                    <input
                        type="number"
                        name="readingGoal"
                        value={formData.readingGoal}
                        onChange={handleInputChange}
                        className={errors.readingGoal ? 'error-input' : ''}
                        placeholder="e.g. 20"
                        min="1"
                    />
                    {errors.readingGoal && <span className="error">{errors.readingGoal}</span>}
                </div>

                <button type="submit" disabled={isLoading}>
                    {isLoading ? 'Creating Account...' : 'Sign Up'}
                </button>
            </form>

            <div className="auth-links">
                <p>Already have an account? <Link to="/login">Log in</Link></p>
                <Link to="/forgot-password">Forgot Password?</Link>
            </div>
        </div>
    );
}
