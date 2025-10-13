export const getUserAchievements = async () => {
    const token = localStorage.getItem("token");
    const res = await fetch('/api/achievement/unlocked', {
        headers: {
            Authorization: `Bearer ${token}`,
        },
    });
    
    if (!res.ok) throw new Error("Failed to load achievements.");
    return await res.json();
};