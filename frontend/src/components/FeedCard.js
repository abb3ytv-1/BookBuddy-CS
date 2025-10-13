export default function FeedCard({ data }) {
    // Check if it's a social post (no bookTitle means social post!)
    const isSocialPost = !data.bookTitle;

    return (
        <div className="feed-card">
            <div className="feed-header">
                <img
                    src={data.profilePicture || "/default-avatar.png"}
                    className="feed-avatar"
                    alt="user avatar"
                />
                <div>
                    <p>
                        <strong>{data.username}</strong>{" "}
                        {isSocialPost
                            ? "shared a post"
                            : `made progress on ${data.bookTitle}`}
                    </p>
                    {data.timestamp && (
                        <p className="feed-timestamp">{data.timestamp}</p>
                    )}
                </div>
            </div>

            <div className="feed-body">
                {isSocialPost ? (
                    <p>{data.content}</p>
                ) : (
                    <>
                        <p>{data.status}</p>
                        <div className="progress-bar">
                            <div
                                className="progress-fill"
                                style={{ width: `${data.progress || 0}%` }}
                            />
                        </div>
                        <p className="feed-comment">{data.comment}</p>

                        <div className="book-card">
                            <img
                                src={data.coverUrl || "/placeholder-book-cover.png"}
                                alt={data.bookTitle}
                            />
                            <div>
                                <h3>{data.bookTitle}</h3>
                                <p>by {data.author}</p>
                                <button className="status-button">
                                    Want to Read
                                </button>
                            </div>
                        </div>
                    </>
                )}
            </div>

            <div className="feed-actions">
                <span>👍 Like</span>
                <span>💬 Comment</span>
            </div>
        </div>
    );
}
