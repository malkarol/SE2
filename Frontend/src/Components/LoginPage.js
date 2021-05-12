import React, { useState } from "react";
import { withRouter, Redirect } from "react-router";
import "../Layout/LoginLayout.css";

export const emailRegex = /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;


function LoginPage() {
    const [login, setLogin] = useState("");
    const [password, setPassword] = useState("");
    const [redirect, setRedirect] = useState(false);
    
    function isEmail(e){
        if (e.length > 0 && emailRegex.test(String(e).toLowerCase()))
            return true;
        return false;  
    }

    const logIn = () => {
        if (login.length > 3 && password.length > 5){
            const alertElem = document.getElementById('error-alert');

            if (isEmail(login)){
                setRedirect(true);
                alertElem.style.display = "none";
            }
            else{
                alertElem.style.display = "block";
            }
        }
    };

    function animateInput(e){
        if (e.target.value.length > 0)
            e.target.className = 'active';
        else
            e.target.className = '';
    }

    if (redirect) {
        return <Redirect push
            to={{
                pathname: "/",
                state: { user: login , userType: 1}
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
                                maxLength = "30"
                                onChange={(e) => setLogin(e.target.value)}
                                onBlur={(e) => animateInput(e)}
                            />
                            <label className="input-label">E-mail</label>
                        </div>
                    </div>
                    <div className="input-row">
                        <svg className="login__icon pass svg-icon" viewBox="0 0 20 20">
                            <path d="M0,20 20,20 20,8 0,8z M10,13 10,16z M4,8 a6,8 0 0,1 12,0" />
                        </svg>
                        <div className="input-group">
                            <input type="password" id="password"
                                maxLength = "30"
                                onChange={(e) => setPassword(e.target.value)}
                                onKeyUp={(e) => {
                                    if (e.key === 'Enter') {
                                        e.preventDefault();
                                        document.querySelector("button").click();
                                    }
                                }}
                                onBlur={(e) => animateInput(e)}
                            />
                            <label className="input-label">Password</label>
                        </div>
                    </div>
                </div>
                <p id="error-alert">E-mail or password you entered doesn't belong to an account.
                Please try again.
                </p>
                <button className="LoginButton" onClick={logIn} type="submit">Sign in</button>
                <a href="#" className="recovery-link">Forgot password?</a>
            </div>
        </div>


    )
}
export default (withRouter(LoginPage));
