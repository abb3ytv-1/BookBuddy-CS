import { useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { toast } from "react-toastify";
import "../index.css";

export default function ResetPassword() {
    const { token } = useParams();
    const [password, setPassword] = useState("");
    const navigate = useNavigate();

    const handleSubmit = async (e) => {
        e.preventDefault();

        if (password.length < 6) {
            toast.error("Password must be at least 6 characters.");
            return;
        }

        try {
            const response = await fetch('/api/auth/reset-password', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ token, password })
            });

            if (!response.ok) {
                const errorText = await response.text();
                throw new Error(errorText || 'Password reset failed');
            }

            toast.success('✅ Password updated successfully');
            navigate('/login');
        } catch (err) {
            toast.error(`❌ ${err.message}`);
        }
    };

    return (
        <div className="auth-page">
            <h1>Set New Password</h1>
            <form onSubmit={handleSubmit}>
                <input
                    type="password"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    placeholder="New Password"
                    required
                />
                <button type="submit">Reset Password</button>
            </form>
        </div>
    );
}
