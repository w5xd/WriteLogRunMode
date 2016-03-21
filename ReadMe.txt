Two Radio Run Mode processor for WriteLog.

Version 2.0.2.0
21 March, 2016

This is a Two Radio run mode keyboard shortcut extension for WriteLog.

It is experimental. But, on the other hand, it has also been
successfully used to sustain very high rates. Use it at your own risk.

Why would I want this?

If ALL Four of these are true:

You run two radios on one PC, and with two Entry windows.
You have an SO2R box that switches your headphones between the two radios.
You want to reduce the number of keystrokes switching between the radios.
You are willing to use a keystroke to designate one or both radios as RUN mode (calling CQ)

Then you MIGHT want to use this add-in.
This add-on takes these actions:
a) starts a CQ when it determines a CQ "radio is idle"
b) switches the headphones/keyboard focus to the "correct" radio.

The reason MIGHT is capitalized is that your definitions of "radio is idle"
and "correct" might not match what the run processor thinks.

The only reasonable way to know the answers is for you to try it.
I include the source code because maybe its close enough that
a little tweak will make it right for you.

It requires WriteLog version 11.24 or later.

To install:

Unzip the distribution zip file somewhere--a temporary location will do.

Double click install.bat

	Did it install? Run WriteLog and look at your Setup/Keyboard Shortcuts/Command to Run.
	Does it have External: start2RadioRunMode and the other External?

Note that the installer does not check its own version number. It
replaces any existing WriteLogRunMode installation.
And note that Writelog can only have one shortcut processor installed at 
a time, so any existing one is disabled when you install this one.

Double click Uninstall to remove it.

********************************

One-time setup considerations.

Message buffers.
The run mode processor ASSUMES that you have set up the following message buffers
in Setup-CW/RTTY/SSB messages:

    Message11--is a CQ -> it sends this message when a CQ rig is otherwise idle
    Message10--sends the CALL and exchange
    Message02--sends the exchange
    Message03--sends QSL QRZ? -> it sends this when you log a QSO a CQ rig.
    Message04--sends MY call
    Message05--send HIS call
    Message06--sends AGN?

Keyboard shortcuts.
The keyboard processor makes the following shortcuts available to be mapped
to the keyboard:

 External:start2RadioRunMode--starts 2-radio CQ. 
          Sends Message11 and then Message11 repeats and alternates on both radios

 External:start1RadioRunMode--starts 1-radio CQ. 
          Sends Message11 and then Message11 repeats on that radio. The other
          radio is assumed to be search and pounce.

 External:startDuelingCQtop
 External:startDuelingCQbottom --starts simplified dueling CQ with first CQ on top Entry
          (or bottom).  Sends Message11 first on that radio, puts keyboard focus (and
		  headphones if not headphones split) on the listening radio. Type anything
		  into either Entry Window and dueling stops with the keyboard (and headphones)
		  set to that Entry.

 External:stop2RadioRunMode--recommend you replace the mapping you have MessageAbortTransmission. 
          Stops transmission in progress as well as stopping the auto-CQ.

 External::EntryClear--recommend you replace the mapping you have EntryClear.
          Duplicates the WriteLog built-in EntryClear and also marks the CQ window as idle.

 External:holdTransmitOn--indicate start of transmission that WriteLog is not automating.
          Asserts PTT and holds all other transmissions until another Message or abort.
          Useful for initiating talking into the transmit microphone or hand-sent CW.
          Invoking this without other intervening state changes instead does endHoldTransmitOn.
 
 External:holdTransmitVOX--same as holdTransmitOn, except does not assert PTT.

 External:endHoldTransmitOn--to indicate end of holdTransmitOn. Releases PTT and
          tells run mode processor you are listening.

 External:Setup--brings up a configuration dialog. 
 
          The run mode processor will  insert Message04 immediately after Message03 
		  if your CALL has not been sent more recently than "maximum seconds between 
		  CALL". Set this negative to never insert Message04.

		  The button "on first CALL letter, holdTransmitVOX" turns ON this feature.
		  When, while run mode is active (after startNRadioRunMode) you type the first 
		  letter into CALL, the holdTransmitVOX state is activated.

You are not required to map all the above to keys, although you had better
map stop2RadioRunMode if you map either of the first two. The holdTransmitOn is
also a matter of preference. You can do without it if all your transmissions are
done from MessageNN shortcuts, or if you can develop the habit of only speaking (SSB)
or sending hand-sent CW when the run mode processor is not going to switch the
transmit focus on you. The endHoldTransmitOn turns out to be unnecessary if you
only use holdTransmitOn to speak the beginning of a message (e.g. the calling
station's CALL) and then always follow immediately by pressing an f-key to complete
the transmission.

Default shortcuts you likely need to change.

The shortcut External:stop2RadioRunMode is mentioned above. You should use it
instead of the built-in MessageAbortTransmission.

Question: I normally map F1 to Message11. What should I do?
It depends on your preference. Here is a recommendation:
    Leave F1 mapped to Message11.
    Map CTRL+F1 to start2RadioRunMode.
    Map ALT+F1 to start1RadioRunMode.
The notion is that initiating the mode is relatively rare. You'll not be pressing
F1 much anymore, but you still can, and the run mode processor accommodates.

keyboard UP/DOWN:
The default settings in WriteLog for UP/DOWN are probably not what you want for 
use with this run processor. They move the transmit focus along with the keyboard
and phones. With the run processor deciding what to transmit next, you will want the
default UP/DOWN to move only the keyboard focus so that correcting entries in 
the Entry window is easy.
 
Recommendation. Use Setup/Keyboard shortcuts in WriteLog remap your UP/DOWN keys:
    EntryKbdFocusUpWithPhones 
    EntryKbdFocusDownWithPhones

********************************

Getting started using it.

Invoke your start2RadioRunMode keyboard shortcut to call CQ on two radios.
    Or
Invoke your start1RadioRunMode shortcut to call CQ on only the current radio.
If you have taken my recommendation above, its CTRL+F1 or SHIFT+F1.

When you hear a response, type it in. The keyboard focus should already
be in the right place. When you want to transmit anything other than a
CQ, then use the appropriate f-key to do so. If the keyboard is on the
wrong radio, you may use UP/DOWN to move it.

When you press ENTER to log a QSO on a CQ radio, the run processor
sends Message03 (QSL+QRZ). Exception: if you hold down SHIFT as you
log the QSO, the sent message is omitted and nothing more happens
on the radio until you send a MessageNN.

The idea is that you type in what you hear, and you press the f-key's
as you would without this automation, with the exception that you don't
normally press the CQ key. You are free to press the CQ key if want and
the run mode processor changes its behavior to match yours.

If you want to transmit using VOX or hand sent CW, use the key mapped 
to holdTransmitOn. End the hold by pressing any message key, or
the External:endHoldTransmitOn key.

If you need everything to stop, use stop2RadioRunMode which most ops would
map to the ESCAPE key. (This shortcut stops the 1 radio version as well.)

********************************

Some details about what the run mode processor does.

The only message it sends is CQ, and then only when nothing else is going on.

WriteLog has three "focus" settings the run mode processor can alter.
    Transmit focus (indicated on-screen by the green circle).
    Keyboard focus (indicated on-screen by the lighter background color)
    Headphone focus (which radio has them--not indicated on screen)

The run mode processor does not touch the headphone focus if you have
the headphones on split. When you change them to one radio or the other,
then it will take over and put them on the other radio when one goes
to transmit. Note that you can have two receiving radios. If the headphones
are not split, the run mode processor generally keeps the headphones on
the radio with the keyboard focus.

The run mode processor is aware of whether you have entered anything in the 
CALL field, and whether you have moved your input cursor away from the CALL 
field and changes its behavior accordingly.

It keeps the keyboard focus on the radio you are listening to, and switches the 
focus to the other radio on any f-key transmission. But it knows about the "red box" 
around the CALL field and delays switching the keyboard focus until you finish the 
red box.

********************************

The WriteLogRunMode directory is the source. Use Visual Studio Express 2010,
If you like, build yourself a new WriteLogRunMode.dll.
Copy it to WriteLog's Programs directory. (or copy it here and run Install.bat)

Good luck.
Wayne, W5XD

********************************
version 2.0.2.0
add setting for Stop aligns xmit&keyboard focus 

version 2.0.1.0
add startDuelingCQtop&bottom

version 1.0.2.2
add holdTransmitVOX

version 1.0.0.9
Work better in SSB mode

version 1.0.0.5
Turns AutoCQ off on start1RadioRunMode or start2RadioRunMode
removed EntryClear shortcut and replaced it without need for user config

version 1.0.0.4
Add holdTransmitOn for non-automated transmission

version 1.0.0.3
update documentation and
changed "WipeQSO" to "EntryClear"

version 1.0.0.2
Add 1 radio mode.
CHANGED F-KEY MAPPING

version 1.0.0.1

Refactor sources.

