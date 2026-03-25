import json
import logging
from dataclasses import dataclass
from dialogue_data import DialogueData

@dataclass
class DialogueManager:
    current_dialogue_index: int = 0
    current_dialogue_letter: str = ""

    dialogue_language_key: str = ""
    speaker_name_key: str = ""
    sentence: str = ""
    speaker_name: str = ""
    language_data = None
    dialogue_started = False
    try:
        with open("static/language.json", mode="r") as language_file:
            language_data = json.load(language_file)
    except FileNotFoundError:
        logging.error(msg="language.json was not found. Check the file path under static", exc_info=True)
        sentence = "Could not file language file."

    def setup_dialogue(self,dialogue_data: DialogueData) -> str:
        self.current_dialogue_index = dialogue_data.dialogue_index
        self.current_dialogue_letter = dialogue_data.dialogue_letter.capitalize()

        self.dialogue_language_key = f"Dialogue_{dialogue_data.dialogue_letter}_Sen_{dialogue_data.dialogue_index}"
        self.speaker_name_key = dialogue_data.speaker_name

        self.speaker_name = self.language_data["en"][self.speaker_name_key]
        try:
            self.sentence = self.language_data["en"][self.dialogue_language_key]
        except KeyError:
            self.sentence = "Sorry that sentence doesn't exist. Enter a different Letter/Index. "
            if dialogue_data.dialogue_index == -1:
                self.sentence += "You did not enter an index. "
            if dialogue_data.dialogue_letter == "":
                self.sentence += "You did not enter a letter."

        return self.sentence

    def start_dialogue(self):
        self.dialogue_started = True

    # called when user presses the continue button
    def get_next_sentence(self) -> str:
        # if the next sentence doesn't exist end the dialogue
        try:
            self.sentence = self.language_data["en"][self.dialogue_language_key]
            self.current_dialogue_index += 1

            self.dialogue_language_key = f"Dialogue_{self.current_dialogue_letter}_Sen_{self.current_dialogue_index}"
            self.sentence = self.language_data["en"][self.dialogue_language_key]
            return self.sentence
        except KeyError:
            self.end_dialogue()
            self.sentence = "Reached the end of this dialogue."
            return self.sentence

    def end_dialogue(self):
        self.dialogue_started = False
        pass