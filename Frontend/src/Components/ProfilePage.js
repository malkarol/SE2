import React, { useState } from "react";
import { withRouter, Redirect } from "react-router";
import "../Layout/MainLayout.css";
import NavBar from "./NavBar";
import VotingPanel from "./VotingPanel";
import VotingPanelCRUD from "./VotingPanelCRUD";

function ProfilePage(props){

    const [activeVoting, setActiveVoting] = useState(null);
    const [answer, setAnswer] = useState(null);
    const ids = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

    return (
        <div className="page">
            <NavBar pageValue={props.isCoordinator == true ? 1 : 0} />
            <div className="main-container">
                <div className="profile-container">
                    <div className="label-row">
                        <h1 className="section-label">Personal information</h1>
                    </div>
                    <div className="databoxes-container">
                        <div className="data-row">
                            <div className="input-box">
                                <label className="input-label">Name</label>
                                <input className="data-inputbox" type="text"/>
                            </div>
                            <div className="input-box">
                                <label className="input-label">Surname</label>
                                <input className="data-inputbox" type="text"/>
                            </div>
                            <div className="input-box">
                                <label className="input-label">Date of birth</label>
                                <input className="data-inputbox" type="text"/>
                            </div>
                            <div className="input-box">
                                <label className="input-label">PESEL</label>
                                <input className="data-inputbox" type="text"/>
                            </div>
                        </div>
                    </div>
                    <div className="label-row">
                        <h1 className="section-label">Contact information</h1>
                    </div>
                    <div className="databoxes-container">
                        <div className="data-row">
                            <div className="input-box">
                                <label className="input-label">Phone</label>
                                <input className="data-inputbox" type="text"/>
                            </div>
                            <div className="input-box">
                                <label className="input-label">E-mail</label>
                                <input className="data-inputbox" type="text"/>
                            </div>
                            <div className="input-box">
                                <label className="input-label">Date of birth</label>
                                <input className="data-inputbox" type="text"/>
                            </div>
                            <div className="input-box">
                                <label className="input-label">Address</label>
                                <input className="data-inputbox" type="text"/>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

        </div>


    );
}

export default ProfilePage;
