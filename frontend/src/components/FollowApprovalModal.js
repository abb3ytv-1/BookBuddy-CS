import Modal from 'react-modal';

function FollowApprovalModal({requests, onApprove, onReject}) {
    return (
        <Modal isOpen={true} contentLabel="Follow Requests">
        <h2>Pending Follow Requests</h2>
        {requests.length === 0 ? (
            <p>No requests right now.</p>
        ) : (
            requests.map(req => (
                <div key={req.id} className='follow-request'>
                    <p>{req.requesterName} wants to follow you.</p>
                    <button onClick={() => onApprove(req.id)}>Approve</button>
                    <button onClick={() => onReject(req.id)}>Reject</button>
                </div>
            ))
        )}
        </Modal>
    )
}