import React, { useState } from "react";
import { withRouter, Redirect } from "react-router";
import "../Layout/LoginLayout.css";

function LoginPage() {
    const [login, setLogin] = useState("");
    const [password, setPassword] = useState("");
    const [redirect, serRedirect] = useState(false);
    const logIn = () => {
        if (login != null && login.length > 0 && password != null && password.length > 0) {
            serRedirect(true);
        }
    };

    if (redirect) {
        return <Redirect
            to={{
                pathname: "/",
                state: { user: login }
            }} />;
    }
    return (
        <div>
            <div className="navbar">
                <div className="MenuItem">Login</div>
                <div className="MenuItem">Login</div>
                <div className="MenuItem">Login</div>
            </div>
            <div className="Login">
                <h3>Log in</h3>
                <input
                    type="text"
                    placeholder="Username"
                    onChange={(e) => setLogin(e.target.value)}></input>
                <input
                    type="password"
                    placeholder="Password"
                    onChange={(e) => setPassword(e.target.value)}></input>
                <button onClick={logIn}>Submit</button>
            </div>
        </div>
    )
}
export default (withRouter(LoginPage));