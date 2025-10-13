import React from 'react';

export default function StatsSection() {
    return (
        <section className="stats">
            <div className="stat">
                <strong>500k+</strong>
                <span>Books Tracked</span>
            </div>
            <div className="stat">
                <strong>150k+</strong>
                <span>Active Readers</span>
            </div>
            <div className="stat">
                <strong>4.8/5</strong>
                <span>Average Rating</span>
            </div>
        </section>
    );
}
