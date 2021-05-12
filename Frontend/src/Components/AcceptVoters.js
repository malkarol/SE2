import React, { useState } from "react";
import { withRouter, Redirect } from "react-router";

function AcceptVoters(props) {

    let voters = ["Karol Malinowski", "Krzysztof Milde", "Cong Nguyen"];

    return (
        <div className="UserPanel">
            {voters.map((value) => {
                return <div className="Accept">
                    <div className="UserAccept">{value}</div>
                    <div className="AcceptReject">
                        <div className="AcceptBtn">&#10003;</div>
                        <div className="RejectBtn">&#10007;</div>
                    </div>
                </div>
            })}
        </div>
    )
}


export default (withRouter(AcceptVoters));