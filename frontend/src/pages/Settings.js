import React, { useState, useEffect, lazy, Suspense } from "react";
import { fetchUserProfile } from "../api/userAPI";
import { useUser } from "../contexts/UserContext";
import { toast } from "react-toastify";

const Sidebar = lazy(() => import("../components/Sidebar"));

export default function SettingsPage() {
    const [bio, setBio] = useState('');
    const [profilePictureUrl, setProfilePictureUrl] = useState('');
    const [preview, setPreview] = useState('');
    const [isPrivate, setIsPrivate] = useState(false);
    const [requireApproval, setRequireApproval] = useState(false);
    const [file, setFile] = useState(null);
    const { reloadUser } = useUser();

    useEffect(() => {
        const fetchData = async () => {
            try {
                const profileRes = await fetch('/api/user/profile', {
                    headers: { Authorization: `Bearer ${localStorage.getItem("token")}` }
                });
                const profile = await profileRes.json();
                setBio(profile.bio || '');
                setProfilePictureUrl(profile.profilePictureUrl || '');
                setPreview(profile.profilePictureUrl || '');

                const privacyRes = await fetch('/api/privacysettings', {
                    headers: { Authorization: `Bearer ${localStorage.getItem("token")}` }
                });
                const privacy = await privacyRes.json();
                setIsPrivate(privacy.isPrivate);
                setRequireApproval(privacy.requireFollowApproval);
            } catch (err) {
                toast.error("Failed to load settings.");
                console.error("Error loading profile/priv settings", err);
            }
        };
        fetchData();

        // Cleanup preview URL on unmount
        return () => {
            if (preview?.startsWith("blob:")) URL.revokeObjectURL(preview);
        };
    }, []);

    const handleImageChange = (e) => {
        const selected = e.target.files[0];
        if (selected) {
            setFile(selected);
            const objectUrl = URL.createObjectURL(selected);
            setPreview(objectUrl);
        }
    };

    const handleSave = async (e) => {
        e.preventDefault();

        let uploadedImageUrl = profilePictureUrl;

        try {
            if (file) {
                const formData = new FormData();
                formData.append("file", file);

                const uploadRes = await fetch("/api/user/upload-profile-picture", {
                    method: "POST",
                    headers: {
                        Authorization: `Bearer ${localStorage.getItem("token")}`
                    },
                    body: formData
                });

                if (!uploadRes.ok) throw new Error("Image upload failed");

                const { url } = await uploadRes.json();
                uploadedImageUrl = url;
            }

            const profileRes = await fetch('/api/user/update-profile', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${localStorage.getItem("token")}`
                },
                body: JSON.stringify({ bio, profilePictureUrl: uploadedImageUrl })
            });

            const privacyRes = await fetch('/api/privacysettings/update', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${localStorage.getItem("token")}`
                },
                body: JSON.stringify({ isPrivate, requireFollowApproval: requireApproval })
            });

            if (profileRes.ok && privacyRes.ok) {
                await reloadUser();
                toast.success("✅ Settings saved successfully!");
            } else {
                throw new Error("Save failed");
            }
        } catch (err) {
            toast.error("❌ " + err.message);
            console.error("Settings save error:", err);
        }
    };

    return (
        <div className="dashboard-container">
            <Suspense fallback={<div>Loading sidebar...</div>}>
                <Sidebar />
            </Suspense>
            <div className="settings-page">
                <h2>Profile Settings</h2>
                <form onSubmit={handleSave}>
                    <label>
                        Bio:
                        <textarea value={bio} onChange={e => setBio(e.target.value)} />
                    </label>
                    <label>
                        Upload Profile Picture:
                        <input type="file" accept="image/*" onChange={handleImageChange} />
                        {preview && <img src={preview} alt="Preview" className="preview-img" />}
                    </label>

                    <h3>Privacy Settings</h3>
                    <label>
                        <input
                            type="checkbox"
                            checked={isPrivate}
                            onChange={() => setIsPrivate(prev => !prev)}
                        />
                        Make my profile private
                    </label>
                    <br />
                    <label>
                        <input
                            type="checkbox"
                            checked={requireApproval}
                            onChange={() => setRequireApproval(prev => !prev)}
                        />
                        Require follow approval
                    </label>

                    <br /><br />
                    <button type="submit">Save Changes</button>
                </form>
            </div>
        </div>
    );
}
