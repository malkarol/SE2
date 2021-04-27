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

const mapStateToProps = (state) => ({ 
    votings: state.votings.votings,
    loading: state.votings.loading,
    error: state.votings.error
});
  
const mapDispatchToProps = (dispatch) => ({
    LoadVotingsAsync: (URL) => dispatch(LoadVotingsAsync(URL))
});

function MainPage(props) {
    const [activeVoting, setActiveVoting] = useState(null);
    const [answer, setAnswer] = useState(null);

    //if (props.location.state == undefined) {
        //return <Redirect to="/login" />;
    //}

    useEffect(() => {props.LoadVotingsAsync(VOTINGS_URL)}, []);

    return (
        <div className="page">
            <NavBar pageValue={props.isCoordinator == true ? 1 : 0} />
            <div className="main-container">
                <div className="MainBoard">
                    <div className="ChoosePanel">
                        <div className="ChooseText">Choose your voting</div>
                        <div className="VotingList">
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
                                                setActiveVoting(value.name);
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
                            if (props.isCoordinator === true){
                                return (<VotingPanelCRUD id={activeVoting}/>)
                            }
                            return (<VotingPanel id={activeVoting} answer={answer} setAnswer={setAnswer} />)
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