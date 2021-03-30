import React, { useState } from "react";
import { withRouter, Redirect } from "react-router";

function MainPage(props) {
    if (props.location.state == undefined) {
        return <Redirect to="/login" />;
    }
    return (
        <div>
            <h3>Hello {props.location.state.user}, welcome to E-voting.</h3>
        </div>
    )
}
export default (withRouter(MainPage));