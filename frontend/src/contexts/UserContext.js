import React, { createContext, useContext, useState, useEffect } from "react";
import { fetchUserProfile } from "../api/userAPI";

const UserContext = createContext();

export function UserProvider({ children }) {
    const [user, setUser] = useState(null);
    const [loading, setLoading] = useState(true);

    const reloadUser = async () => {
        const token = localStorage.getItem('token');
        if (!token) {
            console.warn("No token found. Skipping fetchUserProfile.");
            return;
        }

        try {
            const fetchedUser = await fetchUserProfile();
            setUser(fetchedUser);
        } catch (err) {
            console.error("Failed to reload user", err);
        }
    };

    useEffect(() => {
        reloadUser().finally(() => setLoading(false));
    }, []);

    return (
        <UserContext.Provider value={{ user, reloadUser }}>
            {!loading && children}
        </UserContext.Provider>
    );
}

export function useUser() {
    return useContext(UserContext);
}
