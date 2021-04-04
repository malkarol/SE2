import React, { useState } from "react";
import { withRouter, Redirect } from "react-router";
import "../Layout/MainLayout.css";

function NavBar(props) {
    const subpages = [['My Votings', '/'], ['page2', '#'], ['page3', '#']];
    const [redirect, setRedirect] = useState(false);
    const [redirectPage, setRedirectPage] = useState(null);

    if (redirect) {
        return redirectPage;
    }

    return (
        <div className="navbar">
            {subpages.map((value, index) => {
                const redirect_fun = () => {
                    setRedirectPage(<Redirect push
                        to={{
                            pathname: value[1],
                        }} />);
                    setRedirect(true);
                };
                if (index == props.pageValue) {
                    return <div className="MenuItem Active">{value[0]}</div>
                }
                return <div className="MenuItem" onClick={redirect_fun}>{value[0]}</div>
            })}
        </div>
    )
}
export default (withRouter(NavBar));