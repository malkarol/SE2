import React, { useState } from "react";
import { withRouter, Redirect } from "react-router";
import "../Layout/MainLayout.css";
import NavBar from "./NavBar";
import VotingPanel from "./VotingPanel";
import VotingPanelCRUD from "./VotingPanelCRUD";

function MainPage(props) {
    const [activeVoting, setActiveVoting] = useState(null);
    const [answer, setAnswer] = useState(null);

    //if (props.location.state == undefined) {
        //return <Redirect to="/login" />;
    //}

    const ids = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

    return (
        <div className="page">
            <NavBar pageValue={props.isCoordinator == true ? 1 : 0} />
            <div className="main-container">
                <div className="MainBoard">
                    <div className="ChoosePanel">
                        <div className="ChooseText">Choose your voting</div>
                        <div className="VotingList">
                            {ids.map((value, index) => {
                                if (value == activeVoting) {
                                    return <div className="VotingBar Active">hellohellohello</div>;
                                }
                                else {
                                    const setActiveOnClick = () => {
                                        setActiveVoting(value);
                                        setAnswer(null);
                                    };
                                    return <div className="VotingBar" onClick={() => setActiveOnClick()}>hellohellohello</div>;
                                }
                            })}
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
export default (withRouter(MainPage));