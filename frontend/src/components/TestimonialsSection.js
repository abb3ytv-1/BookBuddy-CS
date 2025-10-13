import React from 'react';

export default function TestimonialsSection() {
    return (
        <section className="testimonials">
            <h2>What Readers Say</h2>
            <div className="testimonials-grid">
                <div className="testimonial-card">
                    <p>"Finally found an app that makes tracking my reading habits enjoyable!"</p>
                    <div className="author">– Sarah, Book Blogger</div>
                </div>
                <div className="testimonial-card">
                    <p>"The community features helped me discover amazing new books."</p>
                    <div className="author">– Mike, Avid Reader</div>
                </div>
            </div>
        </section>
    );
}
