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
        <div className="Login">
            <h3>Log in</h3>
            <input className="LoginInput"
                type="text"
                placeholder="Username"
                onChange={(e) => setLogin(e.target.value)}></input>
            <input className="LoginInput"
                type="password"
                placeholder="Password"
                onChange={(e) => setPassword(e.target.value)}
                onKeyUp={(e) => {
                    if (e.key === 'Enter') {
                        e.preventDefault();
                        document.querySelector("button").click();
                    }
                }}
            ></input>
            <button className="LoginButton" onClick={logIn} type="submit">Submit</button>
        </div>
    )
}
export default (withRouter(LoginPage));