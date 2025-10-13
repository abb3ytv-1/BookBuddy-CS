import Sidebar from './Sidebar';
import '../index.css';

export default function DashboardLayout({ user, children }) {
    return (
        <div className="dashboard-container">
            <Sidebar user={user} />
            <div className="dashboard-main">
                {children}
            </div>
        </div>
    );
}
