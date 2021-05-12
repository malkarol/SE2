import React, { useState } from "react";
import "../Layout/MainLayout.css";
import NavBar from "./NavBar";
import { store } from '../App'
import { ChangeUserType } from '../AppActions/VotingsAction';
import { compose } from "redux";
import { connect } from 'react-redux';
import { withRouter, Redirect } from "react-router";
import UserPanel from "./UserPanel";

const mapStateToProps = (state) => ({
});

const mapDispatchToProps = (dispatch) => ({
    ChangeUserType: () => dispatch(ChangeUserType(2))
});

function ProfilePage(props) {
    props.ChangeUserType(2);
    const [activeVoting, setActiveVoting] = useState(null);
    const [answer, setAnswer] = useState(null);
    const ids = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

    let voter = {
        "name": "Karol", "surname": "Malinowski", "birth": "1920-01-01", "pesel": "40122767190",
        "email": "karol@email.com", "phone": "608 584 122", "country": "Poland", "city": "Warsaw",
        "post": "09-400", "address": "Example 12B"
    };


    return (
        <div className="page">
            <NavBar userType={props.userType} pageValue={1} />
            <div className="main-container">
            <div className="profile-container profile-page">
                <UserPanel voter={voter} />
                </div>
            </div>
        </div>
    );
}




export default compose(
    withRouter,
    connect(mapStateToProps, mapDispatchToProps)
)(ProfilePage);