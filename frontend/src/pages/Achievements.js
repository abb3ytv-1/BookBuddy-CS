import React, { useEffect, useState, lazy, Suspense } from "react";
import "../index.css";

const Sidebar = lazy(() => import("../components/Sidebar"));

export default function Achievements() {
    const [achievements, setAchievements] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchAchievements = async () => {
            try {
                const res = await fetch('/api/achievement/progress', {
                    headers: {
                        Authorization: `Bearer ${localStorage.getItem("token")}`
                    }
                });
                const data = await res.json();
                setAchievements(data);
            } catch (err) {
                console.error("Failed to get achievement progress", err);
            } finally {
                setLoading(false);
            }
        };

        fetchAchievements();
    }, []);

    return (
        <div className="dashboard-container">
            <Suspense fallback={<div>Loading Sidebar...</div>}>
                <Sidebar />
            </Suspense>
            <div className="dashboard-main achievements-page">
                <h1>Achievements</h1>
                {loading ? (
                    <p>Loading achievements...</p>
                ) : (
                    <div className="achievement-grid">
                        {achievements.map((a) => (
                            <div key={a.id} className={`achievement-card ${a.unlocked ? 'unlocked' : 'locked'}`}>
                            <img 
                                loading="lazy"
                                src={a.iconUrl || 'placeholder-icon.png'} 
                                alt={a.title} 
                            />
                            <h3>{a.title}</h3>
                            <p>{a.description}</p>

                            <div className="progress-bar-wrapper">
                                <div className="progress-bar" style={{ width: `${Math.min((a.progressValue / a.goal) * 100, 100)}%` }} />
                            </div>

                            <p>{a.progressValue} / {a.goal}</p>
                            <p className={a.unlocked ? 'unlocked' : 'locked'}>
                                {a.unlocked ? "🏅 Unlocked!" : "🔒 Locked"}
                            </p>
                            </div>
                        ))}
                    </div>
                )}
            </div>
        </div>
    );
}
