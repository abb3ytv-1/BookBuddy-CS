import React from 'react';
import { Link } from 'react-router-dom';

export default function FinalCTASection() {
    return (
        <section className="cta">
            <h2>Start Your Reading Journey Today</h2>
            <Link to="/signup" className="btn">Join Free</Link>
        </section>
    );
}
