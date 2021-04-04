import React, { useState } from "react";
import { withRouter, Redirect } from "react-router";

function VotingPanel(props) {

    const answerIds = [1, 2, 3, 4]

    const sumbitVote = () => {
        alert(props.answer + ' is the answer for question of id ' + props.id);
    }

    return (
        <div className="VotingPanel">
            <div className="Question">QUESTION OF ID {props.id} HERE</div>
            <div className="Answers">
                {answerIds.map((value, index) => {
                    if (props.answer == value) {
                        return (
                            <div className="AnswerBar Active">
                                ANSWER {value} FOR QUESTION OF ID {props.id}
                            </div>);
                    }
                    else {
                        const setAnswerOnClick = () => {
                            props.setAnswer(value);
                        };
                        return (
                            <div className="AnswerBar" onClick={(() => setAnswerOnClick())}>
                                ANSWER {value} FOR QUESTION OF ID {props.id}
                            </div>);
                    }
                })}
            </div>
            <div className="AnswerButton">
                {(() => {
                    if (props.answer == null) {
                        return (
                            <button disabled="true" className="VoteButton">Vote</button>)
                    }
                    else {
                        return (
                            <button className="VoteButton" onClick={sumbitVote}>Vote</button>
                        )
                    }
                })()}

            </div>
        </div>
    )
}


export default (withRouter(VotingPanel));