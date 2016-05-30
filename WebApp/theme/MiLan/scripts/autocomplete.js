/*
---
script: Accessible Autocomplete
license: GNU/GPL license.
description: Accessible version of a Google style autocompleter for MooTools that use AJAX request.
copyright: Copyright (c) Juan Lago D.
authors: [Juan Lago <juanparati[at]gmail[dot]com>, Alex Duschek]

requires:

- core:1.4.0: Element.Event
- core:1.4.0: Element.Event.Delegation
- core:1.4.0:  Request.JSON

provides: [GooCompleter]

...
*/

// MooCompleter class
var GooCompleter = new Class({

	Implements : [Options, Events],

	options : {
		action : 'webservice.php',
		param : 'search',
		method : 'post',
		minlen : 0,
		delay : 1000,

		use_typebox : true,
		clone_typebox : true,
		typebox_offset : {
			x : 2,
			y : 0
		},

		use_listbox : true,
		listbox_offset : {
			x : 1,
			y : 1
		},

		// Custom events
		onAfterComplete : function() {
		}
	},

	blocked : false,
	suggestions : new Array(),

	// AD
	// contains all available suggestions
	allSuggestions : new Array(),

	// saves a value entered by the user
	cachedInput : '',
	// end AD

	/*
	 * Constructor: initialize Constructor
	 *
	 * Add event on formular and perform some stuff, you now, like settings, ...
	 */

	initialize : function(field, options) {

		this.field = document.id(field);

		this.setOptions(options);

		// Disable autocomplete
		this.field.setAttribute('autocomplete', 'off');

		// Disable auto correct and capitalize on IOS.
		this.field.setAttribute('autocapitalize', 'off');
		this.field.setAttribute('autocorrect', 'off');

		// AD
		// WAI ARIA roles and properties
		this.field.setAttribute('role', 'textbox');
		this.field.setAttribute('aria-autocomplete', 'both');
        this.field.setAttribute('aria-live', 'polite');
		this.liveregion = new Element('span', {
			'span' : 'status',
			'aria-live' : 'assertive',
			styles : {
				position: 'absolute',
				clip: 'rect(1px, 1px, 1px, 1px)'
			}
		}).inject(this.field,'after');
		// end AD

		// Setup Typebox
		if(this.options.use_typebox) {
			this.typebox = new Element('div', {
				'class' : 'goocompleter_typebox'
			}).inject(document.body);

			if(this.options.clone_typebox) {

				// Set attributes
				this.typebox.setStyles(this.field.getStyles('font-size', 'font-family', 'font-weight', 'line-height', 'text-align', 'vertical-align', 'width', 'height', 'padding', 'border-top', 'border-left', 'border-right', 'border-bottom', 'letter-spacing'));

				// Reset border color
				this.typebox.setStyle('border-color', 'transparent');

				// Prevent IE 9 padding positioning bug
				/*
				 * if (!Browser.ie9)
				 * this.typebox.setStyles(this.field.getStyles('padding-top'));
				 */

				this.setRelPosition(this.field, this.typebox, this.options.typebox_offset.x, this.options.typebox_offset.y, true);

				window.addEvent('resize', function() {
					this.setRelPosition(this.field, this.typebox, this.options.typebox_offset.x, this.options.typebox_offset.y, true);
				}.bind(this));

				this.field.addEvent('focus', function() {
					this.setRelPosition(this.field, this.typebox, this.options.typebox_offset.x, this.options.typebox_offset.y, true);
				}.bind(this));
				var self = this;
				this.field.addEvent('blur', function() {
					setTimeout( function() {
						if(this.options.use_typebox)
							this.typebox.empty();
						if(this.listbox)
							this.listbox.setStyle('display', 'none');
					}.bind(this), 200);
				}.bind(this));
			}

			// Prevent focus lost
			this.typebox.addEvent('click', function() {
				this.field.focus();
			}.bind(this));

		}

		// AD
		this.allSuggestions = ["spain", "russia", "netherlands", "germany", "france", "usa", "new zealand", "poland", "belgium", "italy", "norway", "canada", "brasil", "argentina", "china", "india", "japan"];
		// end AD

		// Setup Listbox
		if(this.options.use_listbox) {

			this.listbox = new Element('div', {
				'class' : 'goocompleter_listbox'
			}).inject(document.body);

			this.setRelPosition(this.field, this.listbox, this.options.listbox_offset.x, this.options.listbox_offset.y, false);

			window.addEvent('resize', function() {
				this.setRelPosition(this.field, this.listbox, this.options.listbox_offset.x, this.options.listbox_offset.y, false);
			}.bind(this));

			this.field.addEvent('focus', function() {
				this.setRelPosition(this.field, this.listbox, this.options.listbox_offset.x, this.options.listbox_offset.y, false);
			}.bind(this));

			// Add propagated event for list selection
			this.listbox.addEvent('click:relay(li)', function(event, target) {

				event.stop();

				this.field.set('value', target.get('html'));

				if(this.options.use_typebox)
					this.typebox.empty();

				this.listbox.setStyle('display', 'none');

				this.field.focus();

			}.bind(this));

			// Add propagated event for list over
			this.listbox.addEvent('mouseenter:relay(li)', function(event, target) {
				target.addClass('selected');
			});
			// Add navigation events
			this.listbox.addEvent('mouseleave:relay(li)', function(event, target) {
				target.removeClass('selected');
			});
			// Navigate between listbox
			this.field.addEvent('keydown', function(event) {

				// AD

				// modified key events to match WAI ARIA standards
				if(event.key == 'up' || event.key == 'down' || (event.key == "up" && event.altKey) || (event.key == "down" && event.altKey)) {

					if(this.listbox.getStyle('display') != 'none') {
						var selected = this.navigate(event.key);

						if(this.options.use_typebox)
							this.typebox.empty();
						this.field.value = selected.get('html');
					}

					// AD
					else {
						// show all suggestions or the filtered ones
						if(this.field.get('value') == '') {
							this.showSuggestions(this.allSuggestions);
						} else {
							this.getSuggestions();
						}
					}
					// end AD

					event.stop();

				}

				// AD
				// apply value on pressing enter
				if(event.key == "enter") {

					var targets = this.listbox.getElements("li");
					var target = null;

					targets.each(function(item, index) {
						if(item.hasClass("selected")) {
							target = item;
						}
					});
					event.stop();

					this.field.set('value', target.get('html'));

					if(this.options.use_typebox)
						this.typebox.empty();

					this.listbox.setStyle('display', 'none');

				}

				// hide suggestions or revert the auto-completed entry
				if(event.key == "esc") {

					// needed for IE 6-8
					event.stop();

					if(this.listbox.getStyle('display') == 'block') {
						if(this.cachedInput != '') {

							// revert the user input
							this.field.set('value', this.cachedInput);
							this.removeSelection();
						} else {

							// no user input, so hide suggestion list
							this.hideSuggestions();
							this.writeTypebox(null);
						}
						if(this.options.use_typebox)
							this.typebox.empty();
						if(this.listbox)
							this.listbox.setStyle('display', 'none');
					}
				}
				// end AD

			}.bind(this));
		}

		// Retrieve suggestions on keyup
		this.field.addEvent('keyup', function(event) {

			var value = this.field.get('value');

			// Ignore some key events
			if(event.key == 'up' || event.key == 'down' || event.key == 'left' || event.key == 'right' || event.key == 'tab' || event.key == 'esc' || event.key == 'enter')
				return false;

			// AD
			// save user input for reverting
			this.cachedInput = value;
			// end AD

			// Optimize response of typebox
			if(this.options.use_typebox && this.suggestions.length > 0) {
				var cachevalue = this.searchCache(value);

				if(cachevalue == false)
					this.typebox.empty();
				else
					this.writeTypebox(cachevalue);
			}

			if(value.trim() != '' && value.length > this.options.minlen) {

				if(this.blocked)
					clearTimeout(this.timer);
				else {
					this.blocked = true;

					this.timer = this.getSuggestions;
					this.timer.delay(this.options.delay, this);
				}

			} else {
				if(this.options.use_typebox)
					this.typebox.empty();

				if(this.options.use_listbox)
					this.listbox.setStyle('display', 'none');
			}

		}.bind(this));

	},
	/*
	 * Function hideSuggestions Private method
	 *
	 * Hides the suggestion list
	 */
	hideSuggestions : function() {
		this.listbox.setStyle('display', 'none');
	},
	/*
	 * Function: getSuggestions Private method
	 *
	 * Retrieve a list of suggestions
	 */
	getSuggestions : function() {

		if(this.field.get('value').trim() != '') {
			// AD
			// filter all suggestions for the matching suggestions
			this.suggestions = new Array();
			var value = this.field.get("value");

			this.allSuggestions.each( function(item, index) {
				var element = item.substr(0, value.length);
				if(element == value) {
					this.suggestions.push(item);
				}
			}.bind(this));
			this.showSuggestions(this.suggestions);

			// original version used a mysql database to retrieve the values
			// if (this.suggestions[0] != this.field.get('value'))
			// {
			// var request = new Request.JSON({
			// url: this.options.action,
			// method: this.options.method,
			//
			// onSuccess: function(data) {
			// this.suggestions = data;
			// this.showSuggestions(this.suggestions);
			// }.bind(this)
			// });
			//
			// request.send(this.options.param+'='+this.field.get('value'));
			// }
			// end AD
		}

		this.blocked = false;

	},
	/*
	 * Function: showSuggestions Private method
	 *
	 * Show a list of suggestions
	 */
	showSuggestions : function(suggestions) {
		if(suggestions.length > 0) {
			this.liveregion.set('html', suggestions.length + 'autocomplete options');
			var style = 'even';

			// Delete result list
			if(this.options.use_listbox) {
				this.listbox.empty();

				new Element('ul').inject(this.listbox);
				this.listbox.setStyle('display', 'block');
			}

			// Write typebox suggestion
			if(this.options.use_typebox)
				this.writeTypebox(suggestions[0]);

			Object.each(suggestions, function(value) {

				// Show new result list
				if(this.options.use_listbox) {
					new Element('li', {
						html : value,
						'class' : style,
                        'role' : 'option'
					}).inject(this.listbox.getElement('ul'));
					style = style == 'even' ? 'odd' : 'even';
				}

			}.bind(this));

		} else {
			// Hide typebox
			if(this.options.use_typebox)
				this.typebox.empty();

			// Hide new result list
			if(this.options.use_listbox)
				this.listbox.setStyle('display', 'none');

		}

	},
	/*
	 * Function: writeTypebox Private method
	 *
	 * Write a suggestion in the typebox
	 */
	writeTypebox : function(suggestion) {

		if(suggestion != null) {
			var replacement;
			replacement = suggestion.substr(this.field.get('value').length);
			replacement = '<span class="goocompleter_suggestion">' + replacement + '</span>';

			this.typebox.set('html', this.field.get('value') + replacement);
		} else {

			this.typebox.empty();
		}
	},
	/*
	 * Function: searchCache Private method
	 *
	 * Search a suggestion in cache
	 */
	searchCache : function(search) {

		var found = false;

		Object.each(this.suggestions, function(value) {

			if(!found && search.toLowerCase() == value.name.substr(0, search.length).toLowerCase())
				found = value;

		});
		return found;
	},
	/*
	 * Function: navigate Private method
	 *
	 * Navigate between listbox
	 */
	navigate : function(key) {

		var selected = false;
		var nodes = this.listbox.getChildren('ul li');

		nodes.each(function(el) {

			if(!selected && el.hasClass('selected')) {

				el.removeClass('selected');

				if(key == 'up') {
					if(el.getPrevious() == null)
						selected = nodes[nodes.length - 1];
					// getLast() doesn't
					// work!
					else
						selected = el.getPrevious();
				}

				if(key == 'down') {
					if(el.getNext() == null)
						selected = el.getFirst();
					else
						selected = el.getNext();
				}

			}

		});
		// Select First element by default

		if(!selected)
			selected = nodes[0];

		selected.addClass('selected');

		// Detect scroll
		var scrollsize = this.listbox.getScrollSize();
		var size = this.listbox.getSize();

		if(scrollsize.y > size.y) {
			// Move scroll to selected element
			this.listbox.scrollTo(0, selected.getPosition().y - this.listbox.getPosition().y);

			// AD
			// comment
			// console.log(selected.getPosition(this.listbox).y);
			// end AD

		}

		return selected;

	},
	/*
	 * Function: removeSelection Private method
	 *
	 * Removes all optical stylings of the suggestions
	 */
	removeSelection : function() {
		var nodes = this.listbox.getChildren('ul li');

		nodes.each(function(el) {
			if(el.hasClass('selected')) {
				el.removeClass('selected');
			}

		});
	},
	/*
	 * Function: setPosition Private method
	 *
	 * Set a relative position of a element in absolute values
	 */
	setRelPosition : function(field, container, x_offset, y_offset, overlap) {
		var field_position = field.getCoordinates();
		var top = field_position.top + y_offset;
		top += !overlap ? field_position.height : 0;

		container.setStyles({
			top : top,
			left : field_position.left + x_offset
		}); alert(4);
	},
	setRelPosition2: function () {
	    var field_position = this.field.getCoordinates();
	    var top = field_position.top + y_offset;
	    var overlap = false;
	    top += !overlap ? field_position.height : 0;

	    this.container.setStyles({
	        top: top,
	        left: field_position.left + x_offset
	    });
	}
});
