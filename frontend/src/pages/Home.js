import { Link } from 'react-router-dom';
import { FiBook, FiStar, FiUsers, FiCalendar } from 'react-icons/fi';
import { lazy, Suspense } from 'react';
import '../index.css';

// Lazy-loaded sections
const TestimonialsSection = lazy(() => import('../components/TestimonialsSection'));
const StatsSection = lazy(() => import('../components/StatsSection'));
const FinalCTASection = lazy(() => import('../components/FinalCTASections'));

export default function Home() {
    return (
        <div className="container">
            {/* Hero Section */}
            <section className="hero">
                <h1>Welcome To Book Buddy</h1>
                <p>Your reading, organized, gamified, goal-driven, and socially connected.</p>
                <div className="button-group">
                    <Link to="/signup" className="btn">Get Started</Link>
                    <Link to="/login" className="btn">Login</Link>
                </div>
                <img
                    src="/reading-illustration.jpg"
                    alt="A person reading books in a cozy space"
                    className="hero-image"
                />
            </section>

            {/* Features Section */}
            <section className="features">
                <h2>Why Choose Book Buddy?</h2>
                <div className="book-grid">
                    <div className="book-card">
                        <div className="book-info">
                            <FiBook className="feature-icon" />
                            <h3>Track Your Library</h3>
                            <p>Catalog physical and digital books with custom statuses.</p>
                        </div>
                    </div>
                    <div className="book-card">
                        <div className="book-info">
                            <FiStar className="feature-icon" />
                            <h3>Rate & Review</h3>
                            <p>Share your thoughts and discover new favorites.</p>
                        </div>
                    </div>
                    <div className="book-card">
                        <div className="book-info">
                            <FiUsers className="feature-icon" />
                            <h3>Join Community</h3>
                            <p>Connect with fellow readers and share recommendations.</p>
                        </div>
                    </div>
                    <div className="book-card">
                        <div className="book-info">
                            <FiCalendar className="feature-icon" />
                            <h3>Set Goals</h3>
                            <p>Create and track annual reading challenges.</p>
                        </div>
                    </div>
                </div>
            </section>

            {/* Lazy-loaded Sections */}
            <Suspense fallback={<p style={{ textAlign: "center" }}>Loading testimonials...</p>}>
                <TestimonialsSection />
            </Suspense>

            <Suspense fallback={<p style={{ textAlign: "center" }}>Loading stats...</p>}>
                <StatsSection />
            </Suspense>

            <Suspense fallback={<p style={{ textAlign: "center" }}>Loading call-to-action...</p>}>
                <FinalCTASection />
            </Suspense>
        </div>
    );
}
