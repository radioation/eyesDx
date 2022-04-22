import React, { useState, useEffect }  from 'react';

import { StyleSheet, Button, View, SafeAreaView, Text, TextInput } from 'react-native';

import dgram from 'react-native-udp';


const App = () => {
	// setup variables
	const [ currentEvent, setCurrentEvent ] = useState("One");
	const [ currentGroup, setCurrentGroup ] = useState("1");
	const [ currentFiletime, setCurrentFiletime ] = useState("");
	const [ address, setAddress ] = useState("127.0.0.1");

	// Button press updates the current event and group
	const onEventButtonPressed =  ( eventName, group )  => {
		setCurrentEvent(eventName);
		setCurrentGroup(group);
	}

	// 
	const onChangeAddress = text => {
		const address = text;
		setAddress( address );
	};


	// useEffect to setup 1 second timer for event marker messages
	useEffect( () => {
			// setup dgram
			let socket = dgram.createSocket('udp4');
			socket.bind( (Math.random() * 60536) | (0 + 5000) );

			const timer = setInterval( () => {
					const date = new Date();
					setCurrentFiletime ( date.getTime() * 1e4 + 116444736e9 );
					const msg =	`${currentFiletime},${currentEvent},${currentGroup},0`;
					socket.send(msg, 0, msg.length, 5992, address, function(err) {
							if(err) {
							console.log( err );
							}
					});
					}, 1000 ); // every second

			return ( () => { 
					clearInterval( timer );  // clear interval when component unmounts
					socket.close();
			});
	}, [currentEvent,currentGroup, address] ); // Prevent Stale Closure


	return(
			<SafeAreaView style={styles.container}>
				<View style={{ flexDirection: 'row' }}>
					<TextInput style={ styles.textinput }
						onChangeText = { text => onChangeAddress( text ) }
						value={address}
						/>
				</View>

				<Text>{ currentEvent } - { currentGroup } : { currentFiletime } </Text>

				<View>
					<Text style={ styles.buttonGroup1 } onPress={() => onEventButtonPressed( "One", "1" ) } >One</Text>
				</View>

				<View>
					<Text style={ styles.buttonGroup2 } onPress={() => onEventButtonPressed( "Two", "2" ) } >Two</Text>
				</View>

				<View>
					<Text style={ styles.buttonGroup3 } onPress={() => onEventButtonPressed( "Three", "3" ) } >Three</Text>
				</View>

				<View>
					<Text style={ styles.buttonGroup3 } onPress={() => onEventButtonPressed( "Four", "3" ) } >Four</Text>
				</View>

				<View>
					<Text style={ styles.buttonGroup2 } onPress={() => onEventButtonPressed( "Five", "2" ) } >Five</Text>
				</View>

				<View>
					<Text style={ styles.buttonGroup1 } onPress={() => onEventButtonPressed( "Six", "1" ) } >Six</Text>
				</View>

				<View>
					<Text style={ styles.buttonGroup1 } onPress={() => onEventButtonPressed( "Seven", "1" ) } >Seven</Text>
				</View>

			</SafeAreaView>
			)};



const styles = StyleSheet.create({
	container: {
		flex: 1,
		marginHorizontal: 16,
	},
	buttonGroup1: {
		textAlign: 'center',
		backgroundColor: 'lavenderblush',
		marginVertical: 8,
		padding: 10,
		fontSize: 25,
		borderWidth: 1,
	},
	buttonGroup2: {
		textAlign: 'center',
		backgroundColor: 'honeydew',
		marginVertical: 8,
		padding: 10,
		fontSize: 25,
		borderWidth: 1,
	},
	buttonGroup3: {
		textAlign: 'center',
		backgroundColor: 'lavender',
		marginVertical: 8,
		padding: 10,
		fontSize: 25,
		borderWidth: 1,
	},
  textinput: {
		flex: 1,
		height: 40,
		fontSize: 20,
		paddingLeft: 15,
		paddingRight: 15,
		borderColor: 'black',
		backgroundColor: 'azure',
		borderWidth: 1,
		borderRadius:5
  },
});

export default App;
