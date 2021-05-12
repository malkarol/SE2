import React, { useState, useEffect } from "react";
import { compose } from "redux";
import { connect } from 'react-redux';
import { withRouter, Redirect } from "react-router";
import "../Layout/MainLayout.css";
import NavBar from "./NavBar";
import VotingPanel from "./VotingPanel";
import VotingPanelCRUD from "./VotingPanelCRUD";
import { VOTINGS_URL } from '../AppConstants/AppConstants';
import { LoadVotingsAsync } from '../AppActions/VotingsAction';
import { store } from '../App';
import UserPanel from "./UserPanel";


const mapStateToProps = (state) => ({
    votings: state.votings.votings,
    loading: state.votings.loading,
    error: state.votings.error,
    userType: state.votings.userType
});

// const mapDispatchToProps = (dispatch) => ({
//     LoadVotingsAsync: (URL) => dispatch(LoadVotingsAsync(URL))
// });

function ManageVoters(props) {
    const [activeVoter, setActiveVoter] = useState(null);
    console.log(store.getState());

    //if (props.location.state == undefined) {
    //return <Redirect to="/login" />;
    //}

    // useEffect(() => { props.LoadVotingsAsync(VOTINGS_URL) }, []);


    let voters = [
        {
            "name": "Karol", "surname": "Malinowski", "birth": "1920-01-01", "pesel": "40122767190",
            "email": "karol@email.com", "phone": "608 584 122", "country": "Poland", "city": "Warsaw",
            "post": "09-400", "address": "Example 12B"
        },
        {
            "name": "Krzysztof", "surname": "Milde", "birth": "1955-11-12", "pesel": "40342734190",
            "email": "krzysztof@email.com", "phone": "192 514 122", "country": "Poland", "city": "Gdansk",
            "post": "59-442", "address": "Marszalkowska 21/A"
        },
        {
            "name": "Cong", "surname": "Nguyen", "birth": "1998-10-04", "pesel": "98113467190",
            "email": "cong@email.com", "phone": "504 608 913", "country": "Poland", "city": "Poznan",
            "post": "01-588", "address": "Beautiful 51C/13"
        }];

    return (
        <div className="page">
            <NavBar userType={props.userType} pageValue={2} />
            <div className="main-container">
                <div className="MainBoard">
                    <div className="ChoosePanel">
                        <div className="ChooseText">Choose your voter</div>
                        <div className="VotingList">
                            {
                                voters.map((value, index) => {
                                    if (activeVoter) {
                                        if ((value.name + ' ' + value.surname) == activeVoter.name + ' ' + activeVoter.surname) {
                                            return <div className="VotingBar Active">{value.name + ' ' + value.surname}</div>;
                                        }
                                    }
                                    const setActiveOnClick = () => {
                                        setActiveVoter(value);
                                    };
                                    return <div className="VotingBar" onClick={() => setActiveOnClick()}>{value.name + ' ' + value.surname}</div>;
                                })
                            }
                        </div>
                    </div>
                    {(() => {
                        if (activeVoter == null) {
                            return (
                                <div className="ChooseVoting">Click on one of available voters or add a new one...</div>
                            )
                        }
                        else {
                            return (
                                <div className="UserPanel">
                                    <div className="profile-container profile-embedded">
                                        <UserPanel voter={activeVoter} />
                                    </div>
                                </div>)
                        }
                    })()}
                </div>
            </div>
        </div>
    )
}

export default compose(
    withRouter,
    connect(mapStateToProps)
)(ManageVoters);