import * as signalR from '@microsoft/signalr';

let connection;

export function getSignalRConnection() {
    if (!connection) {
        connection = new signalR.HubConnectionBuilder()
            .withUrl("https://localhost:5224/hubs/notifications", {
                accessTokenFactory: () => localStorage.getItem("token"),
                skipNegotiation: true
            })
            .withAutomaticReconnect()
            .build();
    }
    return connection;
}
