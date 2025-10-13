import React from 'react';
import { Link } from 'react-router-dom';

export default function FriendCard({ friend, onUnfollow }) {
    return (
        <div className="friend-card">
            <Link to={`/profile/${friend.userId}`}>
                <img src={friend.profilePictureUrl || '/default-avatar.png'} alt={friend.userName} />
                <p>{friend.userName}</p>
            </Link>
            <button onClick={() => onUnfollow(friend.userId)}>Unfollow</button>
        </div>
    );
}
