from flask import Flask, render_template, request
from jinja2 import TemplateNotFound

from logger import logger

app = Flask(__name__)

INDEX_TEMPLATE_NAME: str = "index.html"

@app.errorhandler(404)
def page_not_found():
    return "This page doesn't exist"

def try_load_template(template_name: str) -> str:
    try:
        return render_template(template_name)
    except TemplateNotFound:
        logger.error(f"Template: {template_name} not found")
        return page_not_found()

@app.route('/')
def home():
    return try_load_template(INDEX_TEMPLATE_NAME)


# @app.route('/get_dialogue', methods=['POST'])
# def process_dialogue():
#     dialogue_letter = request.form['dialogue_letter']
#     dialogue_index = request.form['dialogue_index']
#
#     dialogue = dialogue_manager.get_dialogue(dialogue_letter, dialogue_index)
#
#     if dialogue:
#         return render_template('index.html', dialogue=dialogue)
#     else:
#         return render_template('index.html', error="Dialogue not found.")

if __name__ == '__main__':
    app.run(debug=True)