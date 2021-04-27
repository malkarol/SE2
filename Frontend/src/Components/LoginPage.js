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
        return <Redirect push
            to={{
                pathname: "/",
                state: { user: login }
            }} />;
    }
    return (
        <div className="Page-cont">
            <div className="Login">
                <h3>Welcome to E-Voting</h3>
                <div className="input-container">
                    <div className="input-row">
                        <svg className="login__icon name svg-icon" viewBox="0 0 20 20">
                            <path d="M0,20 a10,8 0 0,1 20,0z M10,0 a4,4 0 0,1 0,8 a4,4 0 0,1 0,-8" />
                        </svg>
                        <div className="input-group">
                            <input autoComplete="new-password" type="text" id="name"
                                onChange={(e) => setLogin(e.target.value)}
                                onBlur={(e) => {
                                    if (e.target.value.length > 0) {
                                        e.target.className = 'active';
                                    } else {
                                        e.target.className = '';
                                    }
                                }}
                            />
                            <label className="input-label">Username</label>
                        </div>
                    </div>
                    <div className="input-row">
                        <svg className="login__icon pass svg-icon" viewBox="0 0 20 20">
                            <path d="M0,20 20,20 20,8 0,8z M10,13 10,16z M4,8 a6,8 0 0,1 12,0" />
                        </svg>
                        <div className="input-group">
                            <input type="password" id="password"
                                onChange={(e) => setPassword(e.target.value)}
                                onKeyUp={(e) => {
                                    if (e.key === 'Enter') {
                                        e.preventDefault();
                                        document.querySelector("button").click();
                                    }
                                }}
                                onBlur={(e) => {
                                    if (e.target.value.length > 0) {
                                        e.target.className = 'active';
                                    } else {
                                        e.target.className = '';
                                    }
                                }}
                            />
                            <label className="input-label">Password</label>
                        </div>
                    </div>

                </div>
                <button className="LoginButton" onClick={logIn} type="submit">Sign in</button>
                <a href="#" className="recovery-link">Forgot password?</a>
            </div>
        </div>


    )

}
export default (withRouter(LoginPage));
