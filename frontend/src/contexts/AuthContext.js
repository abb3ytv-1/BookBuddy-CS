import {createContent, useContent, useEffect, useState } from 'react'
import API from '../api/client'

const AuthContext = createContent()

export const AuthProvider = ({children}) => {
    const [user, setUser] = useState(null);

    const login = async (token) => {
        localStorage.setItem('token', token);
        const response = await API.get('/auth/me')
        setUser(response.data)
    };

    const logout = () => {
        localStorage.removeItem('token')
        setUser(null)
    }

    useEffect(() => {
        const checkAuth = async () => {
            try {
                const response = await API.get('/auth/me')
                setUser(response.data)
            } catch (err) {
                logout()
            }
        }
        checkAuth()
    }, [])

    return (
        <AuthContext.Provider value={{user, login, logout}}>
            {children}
        </AuthContext.Provider>
    )
}

export const useAuth = () => useContent(AuthContext)