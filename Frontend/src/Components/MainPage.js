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


const mapStateToProps = (state) => ({
    votings: state.votings.votings,
    loading: state.votings.loading,
    error: state.votings.error,
    userType: state.votings.userType
});

const mapDispatchToProps = (dispatch) => ({
    LoadVotingsAsync: (URL) => dispatch(LoadVotingsAsync(URL))
});

function MainPage(props) {
    const [activeVoting, setActiveVoting] = useState(null);
    const [answer, setAnswer] = useState(null);
    console.log(store.getState());

    //if (props.location.state == undefined) {
    //return <Redirect to="/login" />;
    //}

    useEffect(() => { props.LoadVotingsAsync(VOTINGS_URL) }, []);

    return (
        <div className="page">
            <NavBar userType={props.userType} pageValue={0} />
            <div className="main-container">
                <div className="MainBoard">
                    <div className="ChoosePanel">
                        <div className="ChooseText">Choose your voting</div>
                        <div className="VotingList">
                            {(() => {
                                if (props.userType == 1) {
                                    return (
                                        <div className="Filters">
                                            <div className="Filter Active">Active</div>
                                            <div className="Filter">Not joined</div>
                                            <div className="Filter">Old</div>
                                            <div className="Filter">Rejected</div>
                                        </div>
                                    )
                                }
                            })()}
                            {
                                props.loading ? <label>...</label> :
                                    (
                                        props.error ? <label>Fetch error...</label> :
                                            props.votings.map((value, index) => {
                                                if (value.name == activeVoting) {
                                                    return <div className="VotingBar Active">{value.name}</div>;
                                                }
                                                else {
                                                    const setActiveOnClick = () => {
                                                        setActiveVoting(value);
                                                        setAnswer(null);
                                                    };
                                                    return <div className="VotingBar" onClick={() => setActiveOnClick()}>{value.name}</div>;
                                                }
                                            })
                                    )
                            }
                        </div>
                    </div>
                    {(() => {
                        if (activeVoting == null) {
                            return (
                                <div className="ChooseVoting">Click on one of available votings...</div>
                            )
                        }
                        else {
                            if (props.userType == 2) {
                                return (<VotingPanelCRUD id={activeVoting} />)
                            }
                            return (<VotingPanel id={activeVoting.id} voting={activeVoting} answer={answer} setAnswer={setAnswer} />)
                        }
                    })()}
                </div>
            </div>
        </div>
    )
}

export default compose(
    withRouter,
    connect(mapStateToProps, mapDispatchToProps)
)(MainPage);