import { createContext, useEffect, useState } from 'react';
import * as signalR from '@microsoft/signalr';

export const NotificationContext = createContext();

export function NotificationProvider({ children }) {
    const [notifications, setNotifications] = useState([]);
    const [unreadCount, setUnreadCount] = useState(0);

    useEffect(() => {
        const token = localStorage.getItem("token");
        if (!token) return;

        const connection = new signalR.HubConnectionBuilder()
            .withUrl("https://localhost:5224/hubs/notifications", {
                accessTokenFactory: () => token
            })
            .withAutomaticReconnect()
            .build();

        let didStart = false;

        connection.on("ReceivedNotification", (notif) => {
            console.log("🔔 Received:", notif);
            setNotifications(prev => [notif, ...prev]);
            setUnreadCount(prev => prev + 1);
        });

        connection.onclose(error => {
            console.warn("[SignalR] Disconnected:", error?.message);
        });

        connection.onreconnecting(error => {
            console.log("[SignalR] Reconnecting:", error?.message);
        });

        connection.onreconnected(connectionId => {
            console.log("[SignalR] Reconnected. Connection ID:", connectionId);
        });

        const startConnection = async () => {
            try {
                if (connection.state === signalR.HubConnectionState.Disconnected) {
                    console.log("[SignalR] Attempting to start...");
                    await connection.start();
                    didStart = true;
                    console.log("✅ SignalR Connected");
                }
            } catch (err) {
                console.error("❌ SignalR Connection Error:", err);
            }
        };

        startConnection();

        return () => {
            if (didStart) {
                connection.stop();
            }
        };
    }, []);

    return (
        <NotificationContext.Provider value={{ notifications, unreadCount, setUnreadCount }}>
            {children}
        </NotificationContext.Provider>
    );
}
