import React, { useState } from "react";
import { withRouter, useHistory } from "react-router";

function MainPage(props) {
    const history = useHistory();
    if (props.location.state == undefined) {
        history.push("/login");
    }
    return (
        <div>
            <h3>Hello {props.location.state.user}, welcome to E-voting.</h3>
        </div>
    )
}
export default (withRouter(MainPage));