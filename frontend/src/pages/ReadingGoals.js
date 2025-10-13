import { useState, useEffect } from 'react';
import { toast } from 'react-toastify';
import DashboardLayout from '../components/DashboardLayout';
import api from '../api/client';

export default function ReadingGoals() {
    const [userData, setUserData] = useState(null);
    const [newGoal, setNewGoal] = useState('');
    const [isEditing, setIsEditing] = useState(false);

    // Fetch user profile and reading goal
    useEffect(() => {
        const fetchProfile = async () => {
            try {
                const { data } = await api.get('/user/profile');
                setUserData(data);
                setNewGoal(data.readingGoal || '');
            } catch {
                toast.error('Failed to load reading goal');
            }
        };
        fetchProfile();
    }, []);

    // Save updated reading goal
    const handleSaveGoal = async () => {
        const goalNum = parseInt(newGoal, 10);
        if (!goalNum || isNaN(goalNum) || goalNum <= 0) {
            toast.error('Please enter a valid positive number');
            return;
        }

        try {
            await api.put('/user/profile', { readingGoal: goalNum });
            setUserData({ ...userData, readingGoal: goalNum });
            setIsEditing(false);
            toast.success('Goal updated!');
        } catch {
            toast.error('Failed to update goal');
        }
    };

    if (!userData) return <div className="loading">Loading...</div>;

    const progress = userData.readingGoal > 0
        ? Math.min(100, ((userData.booksRead / userData.readingGoal) * 100).toFixed(1))
        : 0;

    return (
        <DashboardLayout>
            <h2 className="section-heading">📚 Your Reading Goals</h2>

            <div className="goal-summary">
                <p>
                    You’ve read <strong>{userData.booksRead}</strong> out of{' '}
                    <strong>{userData.readingGoal || '?'}</strong> books.
                </p>

                <div className="progress-bar">
                    <div
                        className="progress-fill"
                        style={{ width: `${progress}%` }}
                        aria-valuenow={progress}
                        aria-valuemin="0"
                        aria-valuemax="100"
                    ></div>
                    <span>{progress}% complete</span>
                </div>

                {!isEditing ? (
                    <button className="edit-goal-btn" onClick={() => setIsEditing(true)}>
                        Edit Goal
                    </button>
                ) : (
                    <div className="edit-goal-form">
                        <input
                            type="number"
                            min="1"
                            value={newGoal}
                            onChange={(e) => setNewGoal(e.target.value)}
                            aria-label="New reading goal"
                        />
                        <button onClick={handleSaveGoal}>Save</button>
                    </div>
                )}
            </div>

            <div className="goal-tips">
                <h3>Tips to Crush Your Goal</h3>
                <ul>
                    <li>📅 Set a consistent reading time each day.</li>
                    <li>📖 Explore genres you haven't tried before.</li>
                    <li>👥 Join a book club for accountability.</li>
                    <li>📚 Track progress in your library section.</li>
                </ul>
            </div>
        </DashboardLayout>
    );
}
