from flask import Flask, render_template, request
from jinja2 import TemplateNotFound
import random

from logger import logger
from dialogue_manager import DialogueManager
from dialogue_data import DialogueData

app = Flask(__name__)

INDEX_TEMPLATE_NAME: str = "index.html"
DIALOGUE_TEMPLATE_NAME = "dialogue.html"

dialogue_manger: DialogueManager = DialogueManager()
dialogue_letters = ['A', 'B', 'C', 'D', 'E', 'F','G','H']
dialogue_datas = [DialogueData(letter,0,"character_Clammy") for letter in dialogue_letters]


@app.errorhandler(404)
def page_not_found(error):
    return "This page doesn't exist"

def try_load_template(template_name: str, **context) -> str:
    try:
        return render_template(template_name_or_list=template_name, **context)
    except TemplateNotFound:
        logger.error(msg=f"Template: {template_name} not found")
        return page_not_found()

@app.route('/')
def home():
    return try_load_template(INDEX_TEMPLATE_NAME)


@app.route('/dialogue', methods=['GET'])
def process_dialogue():
        # if dialogue has started...play the next sentence
        if dialogue_manger.dialogue_started:
            playing_random_dialogue = False
            sentence = dialogue_manger.get_next_sentence()
            logger.debug(msg=f"Playing next sentence")
            return try_load_template(template_name=DIALOGUE_TEMPLATE_NAME, sentence=sentence,
                                     speaker_name=dialogue_manger.speaker_name, random_dialogue=playing_random_dialogue)
        else: # else choose a random dialogue to start
            playing_random_dialogue = True
            random_dialogue_data = random.choice(dialogue_datas)
            sentence = dialogue_manger.setup_dialogue(random_dialogue_data)
            logger.debug(msg=f"Starting a random dialogue")
            dialogue_manger.start_dialogue()
            return try_load_template(template_name=DIALOGUE_TEMPLATE_NAME, sentence=sentence,
                                     speaker_name=dialogue_manger.speaker_name, random_dialogue=playing_random_dialogue)

@app.route('/start_dialogue', methods=['POST'])
def start_dialogue():
    dialogue_manger.end_dialogue()  # end any existing dialogue first

    dialogue_letter = request.form['dialogue_letter'].capitalize()
    dialogue_index = request.form['dialogue_index']
    try:
        dialogue_data = DialogueData(dialogue_letter, int(dialogue_index), "character_Clammy")
    except ValueError:
        # user did not enter an index so set index to -1 which will give them a useful error message.
        dialogue_data = DialogueData(dialogue_letter, -1, "character_Clammy")

    sentence = dialogue_manger.setup_dialogue(dialogue_data)
    logger.info(msg=f"Loading dialogue from user: {sentence}")
    dialogue_manger.start_dialogue()
    return try_load_template(template_name=DIALOGUE_TEMPLATE_NAME, sentence=sentence, speaker_name=dialogue_manger.speaker_name)


if __name__ == '__main__':
    app.run(debug=True)