import { Link, useNavigate } from "react-router-dom";
import { FiUser, FiBook, FiActivity, FiLogOut, FiAward, FiMoon, FiEdit } from "react-icons/fi";
import "../index.css";
import React, { useContext } from 'react';
import { NotificationContext } from '../contexts/NotificationProvider';


export default function Sidebar({ user }) {
    const navigate = useNavigate();

    const { unreadCount } = useContext(NotificationContext);

    const handleLogout = () => {
        localStorage.removeItem("token");
        navigate("/");
    };

    return (
        <div className="dashboard-sidebar">
            <div className="user-info">
                <FiUser className="user-icon" />
                <h2>{user?.userName || "User"}</h2>
                <p>{user?.email}</p>
                <span className="notif-badge">{unreadCount > 0 ? unreadCount : null}</span>
            </div>

            <nav className="dashboard-nav">
                <Link to="/dashboard" className="nav-item">
                    <FiBook /> My Library
                </Link>
                <Link to="/reading-goals" className="nav-item">
                    <FiActivity /> Reading Goals
                </Link>
                <Link to="/achievements" className="nav-item">
                    <FiAward /> Achievements
                </Link>
                <Link to="/friends" className="nav-item">
                    <FiMoon /> Friends
                </Link>
                <Link to="/feed" className="nav-item">
                    <FiActivity /> Feed
                </Link>
                <Link to="/settings" className="nav-item">
                    <FiEdit /> Settings
                </Link>
                <button onClick={handleLogout} className="nav-item">
                    <FiLogOut /> Logout
                </button>
            </nav>
        </div>
    );
}
