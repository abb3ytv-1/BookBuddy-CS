// src/contexts/SignalRProvider.js
import React, { useEffect } from 'react';
import { getSignalRConnection } from '../components/signalRConnection';
import { fetchUserProfile } from '../api/userAPI';
import { useUser } from './UserContext';

export default function SignalRProvider({ children }) {
    const { setUser } = useUser();

    useEffect(() => {
        const connection = getSignalRConnection();

        connection
            .start()
            .then(() => {
                console.log("✅ SignalR connected (global)");

                connection.on("ReceiveProfileUpdate", async () => {
                    console.log("📢 Profile update received");
                    const updated = await fetchUserProfile();
                    setUser(updated);
                });
            })
            .catch((err) => console.error("🚨 SignalR connection failed:", err));

        return () => {
            connection.stop();
            console.log("🛑 SignalR disconnected (global)");
        };
    }, []);

    return children;
}
