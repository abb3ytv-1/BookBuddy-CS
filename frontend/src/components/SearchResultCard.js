import React from 'react';
import { Link } from 'react-router-dom';

export default function SearchResultCard({ user, onFollow }) {
    return (
        <div className="search-result-card">
            <Link to={`/profile/${user.id}`}>
                <img src={user.profilePictureUrl || "/default-avatar.png"} alt={user.userName} />
                <p>{user.userName}</p>
            </Link>
            <button onClick={() => onFollow(user.id)}>Follow</button>
        </div>
    );
}
