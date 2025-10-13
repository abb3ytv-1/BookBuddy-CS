import { useState } from "react";
import { Link } from "react-router-dom";
import { toast } from "react-toastify";
import "../index.css";

export default function ForgotPassword() {
    const [email, setEmail] = useState("");
    const [loading, setLoading] = useState(false);

    const handleSubmit = async (e) => {
        e.preventDefault();

        if (!email || !/\S+@\S+\.\S+/.test(email)) {
            toast.error("Please enter a valid email address.");
            return;
        }

        setLoading(true);

        try {
            const res = await fetch('/api/auth/forgot-password', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ email })
            });

            if (!res.ok) {
                const err = await res.text();
                throw new Error(err || "Failed to send reset email.");
            }

            toast.success("📧 Password reset email sent!");
        } catch (err) {
            toast.error(`❌ ${err.message}`);
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="auth-page">
            <h1>Reset Password</h1>
            <form onSubmit={handleSubmit}>
                <input
                    type="email"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    placeholder="Email"
                    required
                    disabled={loading}
                />
                <button type="submit" disabled={loading}>
                    {loading ? "Sending..." : "Send Reset Link"}
                </button>
            </form>
            <div className="auth-links">
                <Link to="/login">← Back to Login</Link>
            </div>
        </div>
    );
}
